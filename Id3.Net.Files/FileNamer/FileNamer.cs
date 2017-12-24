#region --- License & Copyright Notice ---
/*
Copyright (c) 2005-2012 Jeevan James
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Id3.Frames;

namespace Id3
{
    public sealed class FileNamer
    {
        private readonly string _pattern;
        private readonly Dictionary<string, PropertyInfo> _mapping;

        public FileNamer(string pattern = "{Artists} - {Title}")
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            _mapping = ValidatePatternAndBuildMapping(pattern);
            _pattern = pattern;
        }

        //Ensures that all placeholders in the pattern refer to existing tag frame properties.
        //Then it attempts to mapping dictionary between the frame name and the corresponding frame
        //property, so that multiple renames can be done fast.
        private static Dictionary<string, PropertyInfo> ValidatePatternAndBuildMapping(string pattern)
        {
            //Find all placeholders within the pattern. If there are none, throw an exception.
            MatchCollection framePlaceholderMatches = FramePlaceholderPattern.Matches(pattern);
            if (framePlaceholderMatches.Count == 0)
                throw new ArgumentException(string.Format(Id3FileMessages.MissingPlaceholdersInPattern, pattern));

            //Get all public properties in the tag that derive from Id3Frame.
            PropertyInfo[] frameProperties =
                typeof(Id3Tag).GetProperties().Where(prop => prop.PropertyType.GetTypeInfo().IsSubclassOf(typeof(Id3Frame))).ToArray();

            //Iterate through each placeholder and find the corresponding frame property. If such a
            //property does not exist, throw an exception; otherwise add a mapping from the frame name
            //to the property.
            var mapping = new Dictionary<string, PropertyInfo>(framePlaceholderMatches.Count);
            foreach (Match match in framePlaceholderMatches)
            {
                string matchedFrameName = match.Groups[1].Value.ToLowerInvariant();
                PropertyInfo matchingFrameProperty =
                    frameProperties.FirstOrDefault(frame => frame.Name.Equals(matchedFrameName, StringComparison.OrdinalIgnoreCase));
                if (matchingFrameProperty == null)
                    throw new ArgumentException(string.Format(Id3FileMessages.MissingTagProperty, matchedFrameName));
                if (!mapping.ContainsKey(matchedFrameName))
                    mapping.Add(matchedFrameName, matchingFrameProperty);
            }
            return mapping;
        }

        #region Rename overloads
        public RenameResults Rename(IEnumerable<string> filePaths)
        {
            if (filePaths == null)
                throw new ArgumentNullException("filePaths");
            return new RenameResults(RenameOrSuggest<RenameResult>(filePaths));
        }

        public RenameResults Rename(string directory, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");
            if (!Directory.Exists(directory))
                throw new ArgumentException(string.Format(Id3FileMessages.MissingDirectory, directory), "directory");
            IEnumerable<string> files = Directory.EnumerateFiles(directory, "*.mp3", searchOption);
            return new RenameResults(RenameOrSuggest<RenameResult>(files));
        }
        #endregion

        #region GetSuggestions overloads
        public RenameSuggestions GetSuggestions(IEnumerable<string> filePaths)
        {
            if (filePaths == null)
                throw new ArgumentNullException("filePaths");
            return new RenameSuggestions(RenameOrSuggest<RenameSuggestion>(filePaths));
        }

        public RenameSuggestions GetSuggestions(string directory, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");
            if (!Directory.Exists(directory))
                throw new ArgumentException(string.Format(Id3FileMessages.MissingDirectory, directory), "directory");

            IEnumerable<string> files = Directory.EnumerateFiles(directory, "*.mp3", searchOption);
            return new RenameSuggestions(RenameOrSuggest<RenameSuggestion>(files));
        }
        #endregion

        /// <summary>
        /// Occurs whenever the required tag data is missing for a MP3 file.
        /// </summary>
        public event EventHandler<ResolveMissingDataEventArgs> ResolveMissingData;

        /// <summary>
        /// Occurs before an MP3 file is about to be renamed. Provides the ability to override the
        /// rename operation or to cancel it.
        /// </summary>
        public event EventHandler<RenamingEventArgs> Renaming;

        private IEnumerable<T> RenameOrSuggest<T>(IEnumerable<string> filePaths)
            where T : RenameAction, new()
        {
            foreach (string filePath in filePaths)
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    yield return new T {
                        Directory = filePath,
                        OriginalName = filePath,
                        Status = RenameStatus.Error,
                        ErrorMessage = Id3FileMessages.InvalidFilePath
                    };
                    continue;
                }

                if (!File.Exists(filePath))
                {
                    yield return new T {
                        Directory = filePath,
                        OriginalName = filePath,
                        Status = RenameStatus.Error,
                        ErrorMessage = Id3FileMessages.MissingFile
                    };
                    continue;
                }

                var result = new T {
                    Directory = Path.GetDirectoryName(filePath),
                    OriginalName = Path.GetFileName(filePath)
                };

                using (var mp3 = new Mp3File(filePath))
                {
                    Id3Tag tag = mp3.GetTag(2, 3);
                    if (tag == null)
                    {
                        result.Status = RenameStatus.Error;
                        result.ErrorMessage = Id3FileMessages.MissingId3v23TagInFile;
                        yield return result;
                        continue;
                    }

                    string missingFrameName = null;
                    string newName = FramePlaceholderPattern.Replace(_pattern, match => {
                        string frameName = match.Groups[1].Value;
                        string frameNameKey = frameName.ToLowerInvariant();
                        PropertyInfo frameProperty = _mapping[frameNameKey];
                        var frame = (Id3Frame)frameProperty.GetValue(tag, null);
                        string frameValue = frame.IsAssigned ? frame.ToString() : FireResolveMissingDataEvent(tag, frame, result.OriginalName);
                        if (string.IsNullOrEmpty(frameValue))
                            missingFrameName = frameName;
                        return frameValue;
                    });

                    if (missingFrameName != null)
                    {
                        result.Status = RenameStatus.Error;
                        result.ErrorMessage = string.Format(Id3FileMessages.MissingDataForFrame, missingFrameName);
                        yield return result;
                        continue;
                    }

                    result.NewName = newName + ".mp3";
                    RenamingEventArgs renamingEventResult = FireRenamingEvent(tag, result.OriginalName, result.NewName);
                    if (renamingEventResult.Cancel)
                        result.Status = RenameStatus.Cancelled;
                    else
                        result.NewName = renamingEventResult.NewName;
                    if (result.OriginalName.Equals(result.NewName, StringComparison.Ordinal))
                        result.Status = RenameStatus.CorrectlyNamed;
                    yield return result;
                }
            }
        }

        private static readonly Regex FramePlaceholderPattern = new Regex(@"{(\w+)}");

        private string FireResolveMissingDataEvent(Id3Tag tag, Id3Frame frame, string sourceName)
        {
            EventHandler<ResolveMissingDataEventArgs> resolveMissingData = ResolveMissingData;
            if (resolveMissingData != null)
            {
                var args = new ResolveMissingDataEventArgs(tag, frame, sourceName);
                resolveMissingData(this, args);
                return args.Value;
            }
            return null;
        }

        private RenamingEventArgs FireRenamingEvent(Id3Tag tag, string oldName, string newName)
        {
            EventHandler<RenamingEventArgs> renaming = Renaming;
            var args = new RenamingEventArgs(tag, oldName) {
                NewName = newName
            };
            if (renaming != null)
                renaming(this, args);
            return args;
        }
    }
}
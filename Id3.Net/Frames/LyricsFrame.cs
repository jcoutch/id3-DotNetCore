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

namespace Id3.Frames
{
    public sealed class LyricsFrame : Id3Frame
    {
        private Id3Language _language = Id3Language.eng;
        private string _lyrics;

        public override bool Equals(Id3Frame other)
        {
            var lyricsFrame = other as LyricsFrame;
            return (lyricsFrame != null) && (lyricsFrame.Language == Language) && (lyricsFrame.Description == Description);
        }

        #region Public properties
        public string Description { get; set; }

        public Id3TextEncoding EncodingType { get; set; }

        public override bool IsAssigned
        {
            get { return !string.IsNullOrEmpty(_lyrics); }
        }

        public Id3Language Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public string Lyrics
        {
            get { return _lyrics; }
            set { _lyrics = value; }
        }
        #endregion
    }
}
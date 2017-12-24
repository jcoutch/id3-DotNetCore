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

using Id3.Frames;

namespace Id3
{
    public sealed class ResolveMissingDataEventArgs : EventArgs
    {
        private readonly Id3Tag _tag;
        private readonly Id3Frame _frame;
        private readonly string _sourceName;

        internal ResolveMissingDataEventArgs(Id3Tag tag, Id3Frame frame, string sourceName)
        {
            _tag = tag;
            _frame = frame;
            _sourceName = sourceName;
        }

        public Id3Tag Tag
        {
            get { return _tag; }
        }

        public Id3Frame Frame
        {
            get { return _frame; }
        }

        public string Value { get; set; }

        public string SourceName
        {
            get { return _sourceName; }
        }
    }
}
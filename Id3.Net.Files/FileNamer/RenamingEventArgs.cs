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

namespace Id3
{
    public sealed class RenamingEventArgs : EventArgs
    {
        private readonly Id3Tag _tag;
        private readonly string _oldName;

        internal RenamingEventArgs(Id3Tag tag, string oldName)
        {
            _tag = tag;
            _oldName = oldName;
        }

        public Id3Tag Tag
        {
            get { return _tag; }
        }

        public string OldName
        {
            get { return _oldName; }
        }

        public string NewName { get; set; }

        public bool Cancel { get; set; }
    }
}
﻿#region --- License & Copyright Notice ---
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

using System.Collections.Generic;

namespace Id3.Info
{
    public sealed class InfoProviderInputs
    {
        private readonly IDictionary<string, string> _properties = new Dictionary<string, string>();

        public string FileName { get; set; }

        public int MatchingTolerance { get; set; }

        public IDictionary<string, string> Properties
        {
            get { return _properties; }
        }

        public static InfoProviderInputs Create(string filename)
        {
            return new InfoProviderInputs {
                FileName = filename
            };
        }

        public static InfoProviderInputs Default
        {
            get { return new InfoProviderInputs(); }
        }
    }
}
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

using System.Diagnostics;

namespace Id3.Frames
{
    [DebuggerDisplay("{ToString()}")]
    public abstract class TextFrameBase : Id3Frame
    {
        public override sealed bool Equals(Id3Frame other)
        {
            var text = other as TextFrameBase;
            return (text != null) && (text.GetType() == GetType()) && (text.TextValue == TextValue);
        }

        public override string ToString()
        {
            return IsAssigned ? TextValue : string.Empty;
        }

        public Id3TextEncoding EncodingType { get; set; }

        public override sealed bool IsAssigned
        {
            get { return !string.IsNullOrEmpty(TextValue); }
        }

        internal abstract string TextValue { get; set; }
    }

    public abstract class TextFrameBase<TValue> : TextFrameBase
    {
        public TValue Value { get; set; }

        public static implicit operator TValue(TextFrameBase<TValue> frame)
        {
            return frame.Value;
        }
    }
}
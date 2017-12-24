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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Id3.Frames
{
    [DebuggerDisplay("{ToString()}")]
    public abstract class TextFrame : TextFrameBase<string>
    {
        internal override string TextValue
        {
            get { return Value; }
            set { Value = value; }
        }
    }

    public sealed class AlbumFrame : TextFrame
    {
    }

    public sealed class BandFrame : TextFrame
    {
    }

    public sealed class ConductorFrame : TextFrame
    {
    }

    public sealed class ContentGroupDescriptionFrame : TextFrame
    {
    }

    public sealed class CopyrightFrame : TextFrame
    {
        public override string ToString()
        {
            return IsAssigned ? "Copyright © " + Value : string.Empty;
        }

        internal override string TextValue
        {
            get { return base.TextValue; }
            set
            {
                if (!CopyrightPrefixPattern.IsMatch(value))
                    throw new ArgumentException("Copyright string must start with a 4 digit year and a space", "value");
                base.TextValue = value;
            }
        }

        private static readonly Regex CopyrightPrefixPattern = new Regex(@"^\d{4} ");
    }

    public sealed class CustomTextFrame : TextFrame
    {
    }

    public sealed class EncoderFrame : TextFrame
    {
    }

    public sealed class EncodingSettingsFrame : TextFrame
    {
    }

    public sealed class FileOwnerFrame : TextFrame
    {
    }

    public sealed class GenreFrame : TextFrame
    {
    }

    public sealed class PublisherFrame : TextFrame
    {
    }

    public sealed class SubtitleFrame : TextFrame
    {
    }

    public sealed class TitleFrame : TextFrame
    {
    }
}
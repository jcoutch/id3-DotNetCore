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
    public abstract class UrlLinkFrame : Id3Frame
    {
        public override bool Equals(Id3Frame other)
        {
            var urlLink = other as UrlLinkFrame;
            return (urlLink != null) && (Url == urlLink.Url);
        }

        public override sealed string ToString()
        {
            return IsAssigned ? Url : string.Empty;
        }

        public override sealed bool IsAssigned
        {
            get { return !string.IsNullOrEmpty(Url); }
        }

        public string Url { get; set; }
    }

    public sealed class ArtistUrlFrame : UrlLinkFrame
    {
    }

    public sealed class ArtistUrlFrameList : Id3SyncFrameList<ArtistUrlFrame>
    {
        internal ArtistUrlFrameList(Id3FrameList mainList)
            : base(mainList)
        {
        }
    }

    public sealed class AudioFileUrlFrame : UrlLinkFrame
    {
    }

    public sealed class AudioSourceUrlFrame : UrlLinkFrame
    {
    }

    public sealed class CommercialUrlFrame : UrlLinkFrame
    {
    }

    public sealed class CommercialUrlFrameList : Id3SyncFrameList<CommercialUrlFrame>
    {
        internal CommercialUrlFrameList(Id3FrameList mainList)
            : base(mainList)
        {
        }
    }

    public sealed class CopyrightUrlFrame : UrlLinkFrame
    {
    }

    public sealed class CustomUrlLinkFrame : UrlLinkFrame
    {
        public string Description { get; set; }

        public Id3TextEncoding EncodingType { get; set; }
    }

    public sealed class PaymentUrlFrame : UrlLinkFrame
    {
    }
}
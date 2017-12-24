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
using Id3.Frames;
using Id3.Internal;

namespace Id3.Id3v2
{
    internal sealed partial class Id3v23Handler : Id3v2Handler
    {
        internal override void DeleteTag(Stream stream)
        {
            if (!HasTag(stream))
                return;

            var buffer = new byte[BufferSize];
            int tagSize = GetTagSize(stream);
            int readPos = tagSize, writePos = 0;
            int bytesRead;
            do
            {
                stream.Seek(readPos, SeekOrigin.Begin);
                bytesRead = stream.Read(buffer, 0, BufferSize);
                if (bytesRead == 0)
                    continue;
                stream.Seek(writePos, SeekOrigin.Begin);
                stream.Write(buffer, 0, bytesRead);
                readPos += bytesRead;
                writePos += bytesRead;
            } while (bytesRead == BufferSize);

            stream.SetLength(stream.Length - tagSize);
            stream.Flush();
        }

        internal override byte[] GetTagBytes(Stream stream)
        {
            if (!HasTag(stream))
                return null;

            var sizeBytes = new byte[4];
            stream.Seek(6, SeekOrigin.Begin);
            stream.Read(sizeBytes, 0, 4);
            int tagSize = SyncSafeNumber.DecodeSafe(sizeBytes, 0, 4);

            var tagBytes = new byte[tagSize + 10];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(tagBytes, 0, tagBytes.Length);
            return tagBytes;
        }

        internal override bool HasTag(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var headerBytes = new byte[5];
            stream.Read(headerBytes, 0, 5);

            string magic = AsciiEncoding.GetString(headerBytes, 0, 3);
            return magic == "ID3" && headerBytes[3] == 3;
        }

        internal override Id3Tag ReadTag(Stream stream)
        {
            if (!HasTag(stream))
                return null;

            var tag = new Id3Tag {
                MajorVersion = 2,
                MinorVersion = 3,
                Family = Id3TagFamily.Version2x,
                IsSupported = true,
            };

            stream.Seek(4, SeekOrigin.Begin);
            var headerBytes = new byte[6];
            stream.Read(headerBytes, 0, 6);

            var headerContainer = new Id3v2Header();
            tag.AdditionalData = headerContainer;

            byte flags = headerBytes[1];
            var header = new Id3v2StandardHeader {
                Revision = headerBytes[0],
                Unsyncronization = (flags & 0x80) > 0,
                ExtendedHeader = (flags & 0x40) > 0,
                Experimental = (flags & 0x20) > 0
            };
            headerContainer.Header = header;

            int tagSize = SyncSafeNumber.DecodeSafe(headerBytes, 2, 4);
            var tagData = new byte[tagSize];
            stream.Read(tagData, 0, tagSize);

            int currentPos = 0;
            if (header.ExtendedHeader)
            {
                SyncSafeNumber.DecodeSafe(tagData, currentPos, 4);
                currentPos += 4;

                var extendedHeader = new Id3v2ExtendedHeader {
                    PaddingSize = SyncSafeNumber.DecodeNormal(tagData, currentPos + 2, 4)
                };

                if ((tagData[currentPos] & 0x80) > 0)
                {
                    extendedHeader.Crc32 = SyncSafeNumber.DecodeNormal(tagData, currentPos + 6, 4);
                    currentPos += 10;
                } else
                    currentPos += 6;

                headerContainer.ExtendedHeader = extendedHeader;
            }

            while (currentPos < tagSize && tagData[currentPos] != 0x00)
            {
                string frameId = AsciiEncoding.GetString(tagData, currentPos, 4);
                currentPos += 4;

                int frameSize = SyncSafeNumber.DecodeNormal(tagData, currentPos, 4);
                currentPos += 4;

                var frameFlags = (ushort)((tagData[currentPos] << 0x08) + tagData[currentPos + 1]);
                currentPos += 2;

                var frameData = new byte[frameSize];
                Array.Copy(tagData, currentPos, frameData, 0, frameSize);

                FrameHandler mapping = FrameHandlers[frameId];
                if (mapping != null)
                {
                    Id3Frame frame = mapping.Decoder(frameData);
                    tag.Frames.Add(frame);
                }

                currentPos += frameSize;
            }

            return tag;
        }

        internal override bool WriteTag(Stream stream, Id3Tag tag)
        {
            byte[] tagBytes = GetTagBytes(tag);
            int requiredTagSize = tagBytes.Length;
            if (HasTag(stream))
            {
                int currentTagSize = GetTagSize(stream);
                if (requiredTagSize > currentTagSize)
                    MakeSpaceForTag(stream, currentTagSize, requiredTagSize);
            } else
                MakeSpaceForTag(stream, 0, requiredTagSize);

            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(tagBytes, 0, requiredTagSize);
            stream.Flush();

            return true;
        }

        internal override int MinorVersion
        {
            get { return 3; }
        }

        protected override void BuildFrameHandlers(FrameHandlers mappings)
        {
            mappings.Add<AlbumFrame>("TALB", EncodeAlbum, DecodeAlbum);
            mappings.Add<ArtistsFrame>("TPE1", EncodeArtists, DecodeArtists);
            mappings.Add<ArtistUrlFrame>("WOAR", EncodeArtistUrl, DecodeArtistUrl);
            //mappings.Add<AudioEncryptionFrame>("AENC", null, null);
            mappings.Add<AudioFileUrlFrame>("WOAF", EncodeAudioFileUrl, DecodeAudioFileUrl);
            mappings.Add<AudioSourceUrlFrame>("WOAS", EncodeAudioSourceUrl, DecodeAudioSourceUrl);
            mappings.Add<BandFrame>("TPE2", EncodeBand, DecodeBand);
            mappings.Add<BeatsPerMinuteFrame>("TBPM", EncodeBeatsPerMinute, DecodeBeatsPerMinute);
            mappings.Add<CommentFrame>("COMM", EncodeComment, DecodeComment);
            //mappings.Add<CommercialFrame>("COMR", null, null);
            mappings.Add<CommercialUrlFrame>("WCOM", EncodeCommercialUrl, DecodeCommercialUrl);
            mappings.Add<ComposersFrame>("TCOM", EncodeComposers, DecodeComposers);
            mappings.Add<ConductorFrame>("TPE3", EncodeConductor, DecodeConductor);
            mappings.Add<ContentGroupDescriptionFrame>("TIT1", EncodeContentGroupDescription, DecodeContentGroupDescription);
            mappings.Add<CopyrightFrame>("TCOP", null, null);
            mappings.Add<CopyrightUrlFrame>("WCOP", EncodeCopyrightUrl, DecodeCopyrightUrl);
            mappings.Add<CustomTextFrame>("TXXX", EncodeCustomText, DecodeCustomText);
            mappings.Add<CustomUrlLinkFrame>("WXXX", EncodeCustomUrlLink, DecodeCustomUrlLink);
            mappings.Add<EncoderFrame>("TENC", EncodeEncoder, DecodeEncoder);
            mappings.Add<EncodingSettingsFrame>("TSSE", EncodeEncodingSettings, DecodeEncodingSettings);
            //mappings.Add<EncryptionMethodRegistrationFrame>("ENCR", null, null);
            //mappings.Add<EqualizationFrame>("EQUA", null, null);
            //mappings.Add<EventTimingCodesFrame>("ETCO", null, null);
            mappings.Add<FileOwnerFrame>("TOWN", EncodeFileOwner, DecodeFileOwner);
            mappings.Add<FileTypeFrame>("TFLT", null, null);
            //mappings.Add<GeneralEncapsulationObjectFrame>("GEOB", null, null);
            mappings.Add<GenreFrame>("TCON", EncodeGenre, DecodeGenre);
            //mappings.Add<GroupIdentificationRegistrationFrame>("GRID", null, null);
            //mappings.Add<InitialKeyFrame>("TKEY", null, null);
            //mappings.Add<InvolvedPeopleFrame>("IPLS", null, null);
            //mappings.Add<LanguagesFrame>("TLAN", null, null);
            mappings.Add<LengthFrame>("TLEN", null, null);
            //mappings.Add<LinkedInformationFrame>("LINK", null, null);
            mappings.Add<LyricistsFrame>("TEXT", null, null);
            mappings.Add<LyricsFrame>("USLT", EncodeLyrics, DecodeLyrics);
            //mappings.Add<MediaTypeFrame>("TMED", null, null);
            //mappings.Add<ModifiersFrame>("TPE4", null, null);
            //mappings.Add<MusicCDIdentifierFrame>("MCDI", null, null);
            //mappings.Add<MPEGLocationLookupTableFrame>("MLLT", null, null);
            //mappings.Add<OriginalAlbumFrame>("TOAL", null, null);
            //mappings.Add<OriginalArtistsFrame>("TOPE", null, null);
            //mappings.Add<OriginalFilenameFrame>("TOFN", null, null);
            //mappings.Add<OriginalLyricistFrame>("TOLY", null, null);
            //mappings.Add<OriginalReleaseYearFrame>("TORY", null, null);
            //mappings.Add<OwnershipFrame>("OWNE", null, null);
            //mappings.Add<PartOfASetFrame>("TPOS", null, null);
            mappings.Add<PaymentUrlFrame>("WPAY", EncodePaymentUrl, DecodePaymentUrl);
            mappings.Add<PictureFrame>("APIC", EncodePicture, DecodePicture);
            //mappings.Add<PlayCounterFrame>("PCNT", null, null);
            //mappings.Add<PlaylistDelayFrame>("TDLY", null, null);
            //mappings.Add<PopularimeterFrame>("POPM", null, null);
            //mappings.Add<PositionSynchronizationFrame>("POSS", null, null);
            mappings.Add<PrivateFrame>("PRIV", EncodePrivate, DecodePrivate);
            mappings.Add<PublisherFrame>("TPUB", EncodePublisher, DecodePublisher);
            //mappings.Add<PublisherUrlFrame>("WPUB", null, null);
            //mappings.Add<RadioStationNameFrame>("TRSN", null, null);
            //mappings.Add<RadioStationOwnerFrame>("TRSO", null, null);
            //mappings.Add<RadioStationUrlFrame>("WORS", null, null);
            //mappings.Add<RecommendedBufferSizeFrame>("RBUF", null, null);
            mappings.Add<RecordingDateFrame>("TDAT", EncodeRecordingDate, DecodeRecordingDate);
            //mappings.Add<RecordingDatesFrame>("TRDA", null, null);
            //mappings.Add<RelativeVolumeAdjustmentFrame>("RVAD", null, null);
            //mappings.Add<ReverbFrame>("RVRB", null, null);
            //mappings.Add<SizeFrame>("TSIZ", null, null);
            //mappings.Add<StandardRecordingCodeFrame>("TSRC", null, null);
            mappings.Add<SubtitleFrame>("TIT3", EncodeSubtitle, DecodeSubtitle);
            //mappings.Add<SynchronizedTempoCodesFrame>("SYTC", null, null);
            //mappings.Add<SynchronizedTextFrame>("SYLT", null, null);
            //mappings.Add<TermsOfUseFrame>("USER", null, null);
            //mappings.Add<TimeFrame>("TIME", null, null);
            mappings.Add<TitleFrame>("TIT2", EncodeTitle, DecodeTitle);
            mappings.Add<TrackFrame>("TRCK", EncodeTrack, DecodeTrack);
            //mappings.Add<UniqueFileIDFrame>("UFID", null, null);
            mappings.Add<YearFrame>("TYER", EncodeYear, DecodeYear);
        }

        private byte[] GetTagBytes(Id3Tag tag)
        {
            var bytes = new List<byte>();
            bytes.AddRange(AsciiEncoding.GetBytes("ID3"));
            bytes.AddRange(new byte[] { 3, 0, 0 });
            foreach (Id3Frame frame in tag.Frames)
            {
                if (frame.IsAssigned)
                {
                    FrameHandler mapping = FrameHandlers[frame.GetType()];
                    if (mapping != null)
                    {
                        byte[] frameBytes = mapping.Encoder(frame);
                        bytes.AddRange(AsciiEncoding.GetBytes(GetFrameIdFromFrame(frame)));
                        bytes.AddRange(SyncSafeNumber.EncodeNormal(frameBytes.Length));
                        bytes.AddRange(new byte[] {0, 0});
                        bytes.AddRange(frameBytes);
                    }
                }
            }
            int framesSize = bytes.Count - 6;
            bytes.InsertRange(6, SyncSafeNumber.EncodeSafe(framesSize));
            return bytes.ToArray();
        }

        private static int GetTagSize(Stream stream)
        {
            stream.Seek(6, SeekOrigin.Begin);
            var sizeBytes = new byte[4];
            stream.Read(sizeBytes, 0, 4);
            int tagSize = SyncSafeNumber.DecodeSafe(sizeBytes, 0, 4) + 10;
            return tagSize;
        }

        private static void MakeSpaceForTag(Stream stream, int currentTagSize, int requiredTagSize)
        {
            if (currentTagSize >= requiredTagSize)
                return;

            int increaseRequired = requiredTagSize - currentTagSize;
            var readPos = (int)stream.Length;
            int writePos = readPos + increaseRequired;
            stream.SetLength(writePos);

            var buffer = new byte[BufferSize];
            while (readPos > currentTagSize)
            {
                int bytesToRead = (readPos - BufferSize < currentTagSize) ? readPos - currentTagSize : BufferSize;
                readPos -= bytesToRead;
                stream.Seek(readPos, SeekOrigin.Begin);
                stream.Read(buffer, 0, bytesToRead);
                writePos -= bytesToRead;
                stream.Seek(writePos, SeekOrigin.Begin);
                stream.Write(buffer, 0, bytesToRead);
            }
        }

        private const int BufferSize = 8192;
    }
}
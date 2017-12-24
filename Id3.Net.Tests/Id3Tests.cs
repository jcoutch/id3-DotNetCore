using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using Id3.Frames;

using Xunit;

namespace Id3.Tests
{
    public sealed class Id3Tests
    {
        [Fact]
        public void BasicTests()
        {
            using (var mp3 = new Mp3File(@"D:\SkyDrive\Music\Abba - Fernando.mp3"))
            {
                Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2x);
                Assert.NotNull(tag);
                Assert.True(tag.IsSupported);
                foreach (Id3Frame frame in tag)
                    Console.WriteLine(frame);
            }
        }

        [Fact]
        public void DevTests()
        {
            using (var mp3 = new Mp3File(@"E:\Temp\Audio\BasicTagsWithImage.mp3", Mp3Permissions.ReadWrite))
            {
                mp3.DeleteAllTags();
                var tag = new Id3Tag();
                var frontCover = new PictureFrame {
                    Description = "The Front Cover",
                    MimeType = "image/jpg",
                    PictureType = PictureType.FrontCover
                };
                frontCover.LoadImage(@"E:\Temp\Audio\FrontCover.jpg");
                tag.Pictures.Add(frontCover);
                var fileIcon = new PictureFrame {
                    Description = "The File Icon",
                    MimeType = "image/png",
                    PictureType = PictureType.Other
                };
                fileIcon.LoadImage(@"E:\Temp\Audio\MSN.png");
                tag.Pictures.Add(fileIcon);
                mp3.WriteTag(tag);
                foreach (Id3Frame frame in tag)
                    Console.WriteLine(frame);
            }
        }

        [Fact]
        public void DevTests2()
        {
            using (var mp3 = new Mp3File(@"E:\Temp\Audio\BasicTagsWithImage.mp3"))
            {
                Id3Tag tag = mp3.GetTag(2, 3);
                foreach (PictureFrame frame in tag.OfType<PictureFrame>())
                {
                    Console.WriteLine(frame.EncodingType);
                    Console.WriteLine(frame.PictureType);
                    Console.WriteLine(frame.Description);
                }
            }
        }

        [Fact]
        public void ReproduceTest()
        {
            using (var mp3 = new Mp3File(@"E:\Temp\Audio\BasicTagsWithImage.mp3", Mp3Permissions.ReadWrite))
            {
                mp3.DeleteAllTags();
                var tag = new Id3Tag();
                var pic = new PictureFrame {
                    Description = "The Front Cover",
                    EncodingType = Id3TextEncoding.Iso8859_1
                };
                pic.LoadImage(@"E:\Temp\Audio\FrontCover.jpg");
                tag.Pictures.Add(pic);
                mp3.WriteTag(tag, 2, 3);
            }

            using (var mp3 = new Mp3File(@"E:\Temp\Audio\BasicTagsWithImage.mp3"))
            {
                Id3Tag tag = mp3.GetTag(2, 3);
                PictureFrame pic = tag.OfType<PictureFrame>().First();
                pic.SaveImage(@"E:\Temp\FrontCoverSaved.jpg");
            }
        }

        [Fact]
        public void StubHandlerTests()
        {
            using (var mp3 = new Mp3File(@"E:\Temp\Abba - Fernando.mp3"))
            {
                Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2x);
                Console.WriteLine("{0}.{1}", tag.MajorVersion, tag.MinorVersion);
            }
        }

        [Fact]
        public void SyncListTests()
        {
            var tag = new Id3Tag();

            tag.CustomTexts.Add(new CustomTextFrame {
                Value = "Custom Text 1"
            });
            tag.CustomTexts.Add(new CustomTextFrame {
                Value = "Custom Text 2"
            });
            tag.CustomTexts.Add(new CustomTextFrame {
                Value = "Custom Text 3"
            });
            tag.CustomTexts.Add(new CustomTextFrame {
                Value = "Custom Text 4"
            });

            tag.PrivateData.Add(new PrivateFrame {
                OwnerId = "Private1",
                Data = new byte[] { 1, 2, 3, 4, 5 }
            });
            tag.PrivateData.Add(new PrivateFrame {
                OwnerId = "Private2",
                Data = new byte[] { 1, 2, 3, 4, 5 }
            });
            tag.PrivateData.Add(new PrivateFrame {
                OwnerId = "Private3",
                Data = new byte[] { 1, 2, 3, 4, 5 }
            });
            tag.PrivateData.Add(new PrivateFrame {
                OwnerId = "Private4",
                Data = new byte[] { 1, 2, 3, 4, 5 }
            });

            //tag.RemoveAll<PrivateFrame>(frame => frame.OwnerId.EndsWith("4"));
            tag.Clear();
        }
    }
}
using System.IO;

using Id3.Frames;

namespace Id3
{
    public static class FrameExtensions
    {
        public static void LoadImage(this PictureFrame frame, string filePath)
        {
            frame.PictureData = File.ReadAllBytes(filePath);
        }

        public static void SaveImage(this PictureFrame frame, string filePath)
        {
            File.WriteAllBytes(filePath, frame.PictureData);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LevelEditorPlugin.Library.Image
{
    public static class ImageUtils
    {
        public static void SaveToPNG(int width, int height, byte[] buffer, Stream toStream)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            BitmapSource bitmapSource = BitmapSource.Create(width, height, 96.0, 96.0, System.Windows.Media.PixelFormats.Bgra32, null, buffer, width * 4);

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(toStream);
        }
    }
}

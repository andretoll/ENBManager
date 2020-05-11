using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ENBManager.TestUtils.Utils
{
    public static class TestValues
    {
        private static Random random = new Random();

        public static string GetRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVXYZÅÄÖ0123456789!?&%=_-éèáà";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static BitmapImage GetRandomImage()
        {
            const uint red = 0xFFFF0000, green = 0xFF00FF00;
            var bmp = new WriteableBitmap(800, 600, 96, 96, PixelFormats.Pbgra32, null);
            var data = Enumerable.Range(0, 800 * 600).Select(x => random.NextDouble() > 0.5 ? red : green).ToArray();
            bmp.WritePixels(new Int32Rect(0, 0, 800, 600), data, bmp.BackBufferStride, 0);

            var image = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(stream);
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
            }
            return image;
        }
    }
}

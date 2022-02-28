using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicUnlocker.Services
{
    internal static class DitheringUtils
    {
        

        #region Only for Windows OS
        public static void PerPixelQuantitation(string filePath, int paletteBitSize)
        {
            var pixelsImage = new Bitmap(Image.FromFile(filePath));
            var newImage = new Bitmap(pixelsImage.Width, pixelsImage.Height);

            for (int y = 0; y < pixelsImage.Height; y++)
            {
                for (int x = 0; x < pixelsImage.Width; x++)
                {
                    var currentPixel = pixelsImage.GetPixel(x, y);
                    var r = Math.Round(Convert.ToDouble(paletteBitSize * currentPixel.R / 255)) * 255 / paletteBitSize;
                    var g = Math.Round(Convert.ToDouble(paletteBitSize * currentPixel.G / 255)) * 255 / paletteBitSize;
                    var b = Math.Round(Convert.ToDouble(paletteBitSize * currentPixel.B / 255)) * 255 / paletteBitSize;
                    Color newPixelData = Color.FromArgb((int)r, (int)g, (int)b);
                    newImage.SetPixel(x, y, newPixelData);
                }
            }
            newImage.Save("test_new.jpg");
            Console.WriteLine("Done");
        }

        public static void FloydSteinbergDithering(string filePath, int paletteBitSize)
        {
            ///
            /// Floyd–Steinberg dithering
            ///                 X      7/16
            ///     3 / 16   5 / 16   1 / 16

            double FirstErrorWeight = 7.0 / 16.0;
            double SecondErrorWeight = 3.0 / 16.0;
            double ThirdErrorWeight = 5.0 / 16.0;
            double LastErrorWeight = 1.0 / 16.0;

            var pixelsImage = new Bitmap(Image.FromFile(filePath));
            var newImage = new Bitmap(pixelsImage.Width, pixelsImage.Height);

            for (int y = 0; y < pixelsImage.Height - 1; y++)
            {
                for (int x = 1; x < pixelsImage.Width - 1; x++)
                {

                    var currentPixel = pixelsImage.GetPixel(x, y);
                    var newR = CalculateQuantitation(currentPixel.R, paletteBitSize);
                    var newG = CalculateQuantitation(currentPixel.G, paletteBitSize);
                    var newB = CalculateQuantitation(currentPixel.B, paletteBitSize);
                    Color newPixelData = Color.FromArgb(newR, newG, newB);
                    newImage.SetPixel(x, y, newPixelData);

                    var currentErrors = CalculateError(currentPixel.R, currentPixel.G, currentPixel.B, paletteBitSize);

                    var nextPixel = pixelsImage.GetPixel(x + 1, y);
                    newR = CalculateColorWithError(nextPixel.R, currentErrors.R, FirstErrorWeight);
                    newG = CalculateColorWithError(nextPixel.G, currentErrors.G, FirstErrorWeight);
                    newB = CalculateColorWithError(nextPixel.B, currentErrors.B, FirstErrorWeight);
                    newPixelData = Color.FromArgb(newR, newG, newB);
                    newImage.SetPixel(x + 1, y, newPixelData);

                    nextPixel = pixelsImage.GetPixel(x + 1, y);
                    newR = CalculateColorWithError(nextPixel.R, currentErrors.R, SecondErrorWeight);
                    newG = CalculateColorWithError(nextPixel.G, currentErrors.G, SecondErrorWeight);
                    newB = CalculateColorWithError(nextPixel.B, currentErrors.B, SecondErrorWeight);
                    newPixelData = Color.FromArgb(newR, newG, newB);
                    newImage.SetPixel(x - 1, y + 1, newPixelData);

                    nextPixel = pixelsImage.GetPixel(x + 1, y);
                    newR = CalculateColorWithError(nextPixel.R, currentErrors.R, ThirdErrorWeight);
                    newG = CalculateColorWithError(nextPixel.G, currentErrors.G, ThirdErrorWeight);
                    newB = CalculateColorWithError(nextPixel.B, currentErrors.B, ThirdErrorWeight);
                    newPixelData = Color.FromArgb(newR, newG, newB);
                    newImage.SetPixel(x, y + 1, newPixelData);

                    nextPixel = pixelsImage.GetPixel(x + 1, y);
                    newR = CalculateColorWithError(nextPixel.R, currentErrors.R, LastErrorWeight);
                    newG = CalculateColorWithError(nextPixel.G, currentErrors.G, LastErrorWeight);
                    newB = CalculateColorWithError(nextPixel.B, currentErrors.B, LastErrorWeight);
                    newPixelData = Color.FromArgb(newR, newG, newB);
                    newImage.SetPixel(x + 1, y + 1, newPixelData);
                }
            }
            newImage.Save("test_new.jpg");
            Console.WriteLine("Done");
        }

        private static (int R, int G, int B) CalculateError(int channelRValue, int channelGValue, int channelBValue, int paletteBitSize)
        {
            var channelRError = channelRValue - CalculateQuantitation(channelRValue, paletteBitSize);
            var updatedChannelRValue = channelRValue + channelRError;

            var channelGError = channelGValue - CalculateQuantitation(channelGValue, paletteBitSize);
            var updatedChannelGValue = channelGValue + channelGError;

            var channelBError = channelBValue - CalculateQuantitation(channelBValue, paletteBitSize);
            var updatedChannelBValue = channelBValue + channelBError;

            return (R: (int)updatedChannelRValue, G: (int)updatedChannelGValue, B: (int)updatedChannelBValue);
        }

        private static int CalculateColorWithError(int channelValue, int channelError, double errorWeight)
        {
            var updatedChannelValue = channelValue + channelError * errorWeight;

            return (int)updatedChannelValue;
        }

        private static int CalculateQuantitation(int channelValue, int paletteBitSize)
        {
            return (int)(Math.Round(Convert.ToDouble(paletteBitSize * channelValue / 255)) * 255 / paletteBitSize);
        }
        #endregion
    }
}

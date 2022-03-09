using PicUnlocker.Models;
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
                    var r = (int)(Math.Round(paletteBitSize * currentPixel.R / 255.0) * Math.Round(255.0 / paletteBitSize));
                    var g = (int)(Math.Round(paletteBitSize * currentPixel.G / 255.0) * Math.Round(255.0 / paletteBitSize));
                    var b = (int)(Math.Round(paletteBitSize * currentPixel.B / 255.0) * Math.Round(255.0 / paletteBitSize));
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
            var bufferImage = new IntMap(pixelsImage.Width, pixelsImage.Height);

            for (int y = 0; y < pixelsImage.Height; y++)
            {
                for (int x = 0; x < pixelsImage.Width; x++)
                {
                    var pixel = pixelsImage.GetPixel(x, y);
                    bufferImage.SetLongPixel(x, y, new PixelLongColors(pixel.R, pixel.G, pixel.B));
                }
            }
            


            for (int y = 0; y < pixelsImage.Height - 1; y++)
            {
                for (int x = 1; x < pixelsImage.Width - 1; x++)
                {
                    var currentPixel = bufferImage.GetLongPixel(x, y);
                    var newR = CalculateQuantitation(currentPixel.R, paletteBitSize);
                    var newG = CalculateQuantitation(currentPixel.G, paletteBitSize);
                    var newB = CalculateQuantitation(currentPixel.B, paletteBitSize);
                    
                    newImage.SetPixel(x, y, Color.FromArgb(newR, newB, newG));

                    var currentErrors = CalculateError(currentPixel.R, currentPixel.G, currentPixel.B, paletteBitSize);

                    var nextPixel = bufferImage.GetLongPixel(x + 1, y);
                    newR = CalculateColorWithError(nextPixel.R, currentErrors.R, FirstErrorWeight);
                    newG = CalculateColorWithError(nextPixel.G, currentErrors.G, FirstErrorWeight);
                    newB = CalculateColorWithError(nextPixel.B, currentErrors.B, FirstErrorWeight);

                    bufferImage.SetLongPixel(x + 1, y, new PixelLongColors(newR, newB, newG));

                    nextPixel = bufferImage.GetLongPixel(x - 1, y + 1);
                    newR = CalculateColorWithError(nextPixel.R, currentErrors.R, SecondErrorWeight);
                    newG = CalculateColorWithError(nextPixel.G, currentErrors.G, SecondErrorWeight);
                    newB = CalculateColorWithError(nextPixel.B, currentErrors.B, SecondErrorWeight);

                    bufferImage.SetLongPixel(x - 1, y + 1, new PixelLongColors(newR, newB, newG));

                    nextPixel = bufferImage.GetLongPixel(x, y + 1);
                    newR = CalculateColorWithError(nextPixel.R, currentErrors.R, ThirdErrorWeight);
                    newG = CalculateColorWithError(nextPixel.G, currentErrors.G, ThirdErrorWeight);
                    newB = CalculateColorWithError(nextPixel.B, currentErrors.B, ThirdErrorWeight);

                    bufferImage.SetLongPixel(x, y + 1, new PixelLongColors(newR, newB, newG));

                    nextPixel = bufferImage.GetLongPixel(x + 1, y + 1);
                    newR = CalculateColorWithError(nextPixel.R, currentErrors.R, LastErrorWeight);
                    newG = CalculateColorWithError(nextPixel.G, currentErrors.G, LastErrorWeight);
                    newB = CalculateColorWithError(nextPixel.B, currentErrors.B, LastErrorWeight);

                    bufferImage.SetLongPixel(x + 1, y + 1, new PixelLongColors(newR, newB, newG));
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
            if(updatedChannelBValue > 255 || updatedChannelGValue > 255 || updatedChannelRValue > 255)
            {
                throw new Exception("pixel data overflow");
            }
            return (R: (int)updatedChannelRValue, G: (int)updatedChannelGValue, B: (int)updatedChannelBValue);
        }

        private static int CalculateColorWithError(int channelValue, int channelError, double errorWeight)
        {
            var updatedChannelValue = channelValue + channelError * errorWeight;

            return (int)updatedChannelValue;
        }

        private static int CalculateQuantitation(int channelValue, int paletteBitSize)
        {
            return (int)(Math.Round(paletteBitSize * channelValue / 255.0) * Math.Round(255.0 / paletteBitSize));
        }
        #endregion
    }
}

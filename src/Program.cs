using System;
using System.Drawing;
using System.IO;
using System.Text;
using PicUnlocker.Services;

namespace PicUnlocker
{
    class Program
    {
        static void Main(string[] args)
        {
            var pictureName = string.Empty;
            int paletteSize = 0;

            bool needToQuantitize = false;
            bool needToDithering = false;

            if (args.Length > 1)
            {
                pictureName = args[0];
                int.TryParse(args[1], out paletteSize);
                if ( args[2] == "-q")
                {
                    needToQuantitize = true;
                }
                if (args[2] == "-d")
                {
                    needToDithering = true;
                }
            }

            if (args.Length != 0)
            {
               pictureName = args[0];
            }           
            else
            {
                pictureName = Environment.GetEnvironmentVariable("PIC_NAME");
            }
            if (string.IsNullOrEmpty(pictureName))
            {
                Console.Error.WriteLine("No picture name given");
                return;
            }

            var dirPath = Directory.GetCurrentDirectory();
            var filePath = dirPath + $"/etc/{pictureName}";
            if (!Directory.Exists(dirPath))
            {
                Console.WriteLine("Error. Incorrect path");
                return;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error. File not exist");
                return;
            }

            //var enviromentSystem = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            if (needToQuantitize)
            {
                PerPixelQuantitation(filePath, paletteSize);
                
            }
            else if(needToDithering)
            {
                FloydSteinbergDithering(filePath, paletteSize);
            }
            else
            {
                UnwrapPicture(filePath);
            }
        }

        static void UnwrapPicture(string filePath)
        {
            var pngUnwrapper = new PngUnwrapper();

            var picBytes = pngUnwrapper.GetBytesFromPicture(filePath);
            var picSignatureBytes = pngUnwrapper.GetHeaderBytes(picBytes);
            var picTailBytes = pngUnwrapper.GetTailBytes(picBytes);
            var chunks = pngUnwrapper.RemoveHeaderBytes(pngUnwrapper.RemoveHeaderBytes(picBytes));

            Console.WriteLine($"Total bytes: {picBytes.Length}\n");
            Console.WriteLine($"Signature: {BitConverter.ToString(picSignatureBytes).Replace("-", " ")}\n");
            Console.WriteLine($"Main chunks: {BitConverter.ToString(chunks).Replace("-", " ")}\n");
            Console.WriteLine($"Tail: {BitConverter.ToString(picTailBytes).Replace("-", " ")}\n");

           
        }

        #region Only for Windows OS
        static void PerPixelQuantitation(string filePath, int paletteBitSize)
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

        static void FloydSteinbergDithering(string filePath, int paletteBitSize)
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
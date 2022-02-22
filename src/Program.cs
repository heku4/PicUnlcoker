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

            double FirstErrorWeight = 7 / 16;
            double SecondErrorWeight = 3 / 16;
            double ThirdErrorWeight = 5 / 16;
            double LastErrorWeight = 1 / 16;

            var pixelsImage = new Bitmap(Image.FromFile(filePath));
            var newImage = new Bitmap(pixelsImage.Width, pixelsImage.Height);

            for (int y = 0; y < pixelsImage.Height - 1; y++)
            {
                for (int x = 1; x < pixelsImage.Width - 1; x++)
                {

                    var currentPixel = pixelsImage.GetPixel(x, y);
                    {
                        var newColors = CalculateChannelValueWithError(currentPixel.R, currentPixel.G, currentPixel.B, paletteBitSize, FirstErrorWeight);
                        Color newPixelData = Color.FromArgb(newColors.R, newColors.G, newColors.B);
                        newImage.SetPixel(x + 1, y, newPixelData);
                    }
                    {
                        var newColors = CalculateChannelValueWithError(currentPixel.R, currentPixel.G, currentPixel.B, paletteBitSize, SecondErrorWeight);
                        var newPixelData = Color.FromArgb(newColors.R, newColors.G, newColors.B);
                        newImage.SetPixel(x - 1, y + 1, newPixelData);
                    }
                    
                    {
                        var newColors = CalculateChannelValueWithError(currentPixel.R, currentPixel.G, currentPixel.B, paletteBitSize, ThirdErrorWeight);
                        var newPixelData = Color.FromArgb(newColors.R, newColors.G, newColors.B);
                        newImage.SetPixel(x, y + 1, newPixelData);
                    }
                    
                    {
                        var newColors = CalculateChannelValueWithError(currentPixel.R, currentPixel.G, currentPixel.B, paletteBitSize, LastErrorWeight);
                        var newPixelData = Color.FromArgb(newColors.R, newColors.G, newColors.B);
                        newImage.SetPixel(x + 1, y + 1, newPixelData);
                    }
                    

                }
            }
            newImage.Save("test_new.jpg");
            Console.WriteLine("Done");
        }

        private static (int R, int G, int B) CalculateChannelValueWithError(int channelRValue, int channelGValue, int channelBValue, int paletteBitSize, double errorWeight)
        {
            var channelRError = channelRValue - CalculateErrorOnChannel(channelRValue, paletteBitSize);
            var updatedChannelRValue = channelRValue + channelRError * errorWeight; 

            var channelGError = channelGValue - CalculateErrorOnChannel(channelGValue, paletteBitSize);
            var updatedChannelGValue = channelGValue + channelGError * errorWeight; 

            var channelBError = channelBValue - CalculateErrorOnChannel(channelBValue, paletteBitSize);
            var updatedChannelBValue = channelBValue + channelBError * errorWeight;

           // Console.WriteLine($"R: {(int)updatedChannelRValue}, G: {(int)updatedChannelGValue}, B: {(int)updatedChannelBValue}");
            return (R: (int)updatedChannelRValue, G: (int)updatedChannelGValue, B: (int)updatedChannelBValue);
        }

        private static int CalculateErrorOnChannel(int channelValue, int paletteBitSize)
        {
            return (int)(Math.Round(Convert.ToDouble(paletteBitSize * channelValue / 255)) * 255 / paletteBitSize);
        }
        #endregion
    }
}
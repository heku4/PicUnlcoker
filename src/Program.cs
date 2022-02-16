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
            //var enviromentSystem = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            UnwrapPicture(pictureName);
        }

        static void UnwrapPicture(string picName)
        {
            var dirPath = Directory.GetCurrentDirectory();
            var filePath = dirPath + $"/etc/{picName}";
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

            var pngUnwrapper = new PngUnwrapper();

            var picBytes = pngUnwrapper.GetBytesFromPicture(filePath);
            var picSignatureBytes = pngUnwrapper.GetHeaderBytes(picBytes);
            var picTailBytes = pngUnwrapper.GetTailBytes(picBytes);
            var chunks = pngUnwrapper.RemoveHeaderBytes(pngUnwrapper.RemoveHeaderBytes(picBytes));

            Console.WriteLine($"Total bytes: {picBytes.Length}\n");
            Console.WriteLine($"Signature: {BitConverter.ToString(picSignatureBytes).Replace("-", " ")}\n");
            Console.WriteLine($"Main chunks: {BitConverter.ToString(chunks).Replace("-", " ")}\n");
            Console.WriteLine($"Tail: {BitConverter.ToString(picTailBytes).Replace("-", " ")}\n");

            FloydSteinbergDithering(filePath);
        }

        static void FloydSteinbergDithering(string filePath)
        {
            ///
            /// Floyd–Steinberg dithering
            /// Only for windows os
            ///                 X      7/16
            ///     3 / 16   5 / 16   1 / 16

            var pixelsImage = new Bitmap(Image.FromFile(filePath));
            var newImage = new Bitmap(pixelsImage.Width, pixelsImage.Height);
            var bitPaleteSize = 1;

            for (int y = 0; y < pixelsImage.Height; y++)
            {
                for (int x = 0; x < pixelsImage.Width; x++)
                {
                    var currentPixel = pixelsImage.GetPixel(x, y);
                    var r = Math.Round(Convert.ToDouble(currentPixel.R / 255)) * 255;
                    var g = Math.Round(Convert.ToDouble(currentPixel.G / 255)) * 255;
                    var b = Math.Round(Convert.ToDouble(currentPixel.B / 255)) * 255;
                    Color newPixelData = Color.FromArgb((int)r, (int)g, (int)b);
                    newImage.SetPixel(x, y, newPixelData);
                }
            }
            newImage.Save("test_new.jpg");
        }

        static void CalculateDitheringError(uint x, uint y)
        {

        }
    }
}
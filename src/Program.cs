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
            bool needToUnwrap = true;
            if (args.Length > 1)
            {
                pictureName = args[0];
                int.TryParse(args[1], out paletteSize);
                if ( args[2] == "-d")
                {
                    needToUnwrap = false;
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
            if (needToUnwrap)
            {
                UnwrapPicture(filePath);

            }
            else
            {
                FloydSteinbergDithering(filePath, paletteSize);
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

        static void FloydSteinbergDithering(string filePath, int bitPaletteSize)
        {
            ///
            /// Floyd–Steinberg dithering
            /// Only for windows os
            ///                 X      7/16
            ///     3 / 16   5 / 16   1 / 16

            var pixelsImage = new Bitmap(Image.FromFile(filePath));
            var newImage = new Bitmap(pixelsImage.Width, pixelsImage.Height);

            for (int y = 0; y < pixelsImage.Height; y++)
            {
                for (int x = 0; x < pixelsImage.Width; x++)
                {
                    var currentPixel = pixelsImage.GetPixel(x, y);
                    var r = Math.Round(Convert.ToDouble(bitPaletteSize * currentPixel.R / 255)) * 255 / bitPaletteSize;
                    var g = Math.Round(Convert.ToDouble(bitPaletteSize * currentPixel.G / 255)) * 255 / bitPaletteSize;
                    var b = Math.Round(Convert.ToDouble(bitPaletteSize * currentPixel.B / 255)) * 255 / bitPaletteSize;
                    Color newPixelData = Color.FromArgb((int)r, (int)g, (int)b);
                    newImage.SetPixel(x, y, newPixelData);
                }
            }
            newImage.Save("test_new.jpg");
            Console.WriteLine("Done");
        }

        static void CalculateDitheringError(uint x, uint y)
        {

        }
    }
}
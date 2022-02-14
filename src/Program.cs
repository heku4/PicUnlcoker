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
            var pictureName = Environment.GetEnvironmentVariable("PIC_NAME");
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
            var bitPaleteSize = 1;

            for (int y = 0; y < pixelsImage.Height; y++)
            {
                for (int x = 0; x < pixelsImage.Width; x++)
                {
                    var currentPixel = pixelsImage.GetPixel(x, y);
                    Console.WriteLine($"Current position: {x}, {y}");

                    var output = $"R:{currentPixel.R.ToString()}|G:{currentPixel.G.ToString()}|B:{currentPixel.B.ToString()}";

                    Console.WriteLine(output);
                }
            }            
        }

        static void CalculateDitheringError(uint x, uint y)
        {

        }
    }
}
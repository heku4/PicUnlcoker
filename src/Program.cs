using System;
using System.Drawing;
using System.IO;
using System.Text;
using PicUnlocker.Services;

namespace PicUnlocker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var pictureName = string.Empty;
            var paletteSize = 0;

            var needToQuantitize = false;
            var needToDithering = false;

            if (args.Length > 1)
            {
                pictureName = args[0];
                int.TryParse(args[1], out paletteSize);
                if (args[2] == "-q") needToQuantitize = true;
                if (args[2] == "-d") needToDithering = true;
            }

            if (args.Length != 0)
                pictureName = args[0];
            else
                pictureName = Environment.GetEnvironmentVariable("PIC_NAME");
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
                DitheringUtils.PerPixelQuantitation(filePath, paletteSize);
            else if (needToDithering)
                DitheringUtils.FloydSteinbergDithering(filePath, paletteSize);
            else
                UnwrapPicture(filePath);
        }

        private static void UnwrapPicture(string filePath)
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
    }
}
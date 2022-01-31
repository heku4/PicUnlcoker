using System;
using System.IO;
using System.Text;

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
            var filePath = dirPath + $"/etc/{picName}.png";
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
///180x215
/// каждый байт можно закодировать как два хекс числа
/// то-есть 4 бита одним числом в шестнадцатиричной системе

            var fs = File.ReadAllBytes(filePath);
            Console.Write($"Bytes count: {fs.Length}\n");

            
            
            var dataFromPic = BitConverter.ToString(fs).Replace("-", " ");;
            Console.Write($"Data: {dataFromPic}\n");

        }
    }
}
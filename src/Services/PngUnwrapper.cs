using System.Collections;
using System.IO;
using System.Linq;

namespace PicUnlocker.Services
{
    public class PngUnwrapper: IPictureUnwrapper
    {
        private const int SIGNATURE_LENGTH = 8;
        private const int END_CHUNK_LENGTH = 4;

        public PngUnwrapper()
        {
            
        }
        public byte[] GetBytesFromPicture(string pictureFilePath) =>  File.ReadAllBytes(pictureFilePath);
        
        public byte[] RemoveHeaderBytes(string pictureFilePath)
        {
            var fs = File.ReadAllBytes(pictureFilePath);
            return fs.Skip(SIGNATURE_LENGTH).ToArray();
        }

        public byte[] RemoveHeaderBytes(byte[] pictureInBytes) => pictureInBytes.Skip(SIGNATURE_LENGTH).ToArray();

        public byte[] RemoveTailBytes(string pictureFilePath)
        {
            var fs = File.ReadAllBytes(pictureFilePath);
            return fs.SkipLast(END_CHUNK_LENGTH).ToArray();
        }

        public byte[] RemoveTailBytes(byte[] pictureInBytes) => pictureInBytes.SkipLast(END_CHUNK_LENGTH).ToArray();
    }
}
using System.Collections;
using System.IO;
using System.Linq;

namespace PicUnlocker.Services
{
    public class PngUnwrapper : IPictureUnwrapper
    {
        /// <summary>
        /// PNG standard https://www.w3.org/TR/2003/REC-PNG-20031110/
        /// </summary>
        private const int SIGNATURE_LENGTH = 8;

        private const int END_CHUNK_LENGTH = 4;

        public PngUnwrapper()
        {
        }

        #region IPictureUnwrapper realisation

        public byte[] GetBytesFromPicture(string pictureFilePath)
        {
            return File.ReadAllBytes(pictureFilePath);
        }

        public byte[] GetHeaderBytes(string pictureFilePath)
        {
            return File.ReadAllBytes(pictureFilePath).Take(SIGNATURE_LENGTH).ToArray();
        }

        public byte[] GetHeaderBytes(byte[] pictureInBytes)
        {
            return pictureInBytes.Take(SIGNATURE_LENGTH).ToArray();
        }

        public byte[] RemoveHeaderBytes(string pictureFilePath)
        {
            var fs = File.ReadAllBytes(pictureFilePath);
            return fs.Skip(SIGNATURE_LENGTH).ToArray();
        }

        public byte[] RemoveHeaderBytes(byte[] pictureInBytes)
        {
            return pictureInBytes.Skip(SIGNATURE_LENGTH).ToArray();
        }

        public byte[] RemoveTailBytes(string pictureFilePath)
        {
            var fs = File.ReadAllBytes(pictureFilePath);
            return fs.SkipLast(END_CHUNK_LENGTH).ToArray();
        }

        public byte[] RemoveTailBytes(byte[] pictureInBytes)
        {
            return pictureInBytes.SkipLast(END_CHUNK_LENGTH).ToArray();
        }

        public byte[] GetTailBytes(string pictureFilePath)
        {
            return File.ReadAllBytes(pictureFilePath).Take(END_CHUNK_LENGTH).ToArray();
        }

        public byte[] GetTailBytes(byte[] pictureInBytes)
        {
            return pictureInBytes.Take(END_CHUNK_LENGTH).ToArray();
        }

        #endregion

        #region ByPixel unwrapping

        #endregion
    }
}
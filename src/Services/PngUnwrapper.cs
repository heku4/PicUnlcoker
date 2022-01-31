using System.IO;

namespace PicUnlocker.Services
{
    public class PngUnwrapper: IPictureUnwrapper
    {
        public byte[] GetBytesFromPicture(FileStream pictureStream)
        {
            throw new System.NotImplementedException();
        }

        public byte[] RemoveHeaderBytes(FileStream pictureStream)
        {
            throw new System.NotImplementedException();
        }

        public byte[] RemoveHeaderBytes(byte[] pictureInBytes)
        {
            throw new System.NotImplementedException();
        }

        public byte[] RemoveTailBytes(FileStream pictureStream)
        {
            throw new System.NotImplementedException();
        }

        public byte[] RemoveTailBytes(byte[] pictureInBytes)
        {
            throw new System.NotImplementedException();
        }
    }
}
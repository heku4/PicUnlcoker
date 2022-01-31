using System.IO;

namespace PicUnlocker.Services
{
    public interface IPictureUnwrapper
    {
        public byte[] GetBytesFromPicture(FileStream pictureStream);
        
        public byte[] RemoveHeaderBytes(FileStream pictureStream);
        
        public byte[] RemoveHeaderBytes(byte[] pictureInBytes);
        
        public byte[] RemoveTailBytes(FileStream pictureStream);
        
        public byte[] RemoveTailBytes(byte[] pictureInBytes);
    }
}
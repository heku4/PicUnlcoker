using System.IO;

namespace PicUnlocker.Services
{
    public interface IPictureUnwrapper
    {
        public byte[] GetBytesFromPicture(string pictureFilePath);
        
        public byte[] RemoveHeaderBytes(string pictureFilePath);
        
        public byte[] RemoveHeaderBytes(byte[] pictureInBytes);
        
        public byte[] RemoveTailBytes(string pictureFilePath);
        
        public byte[] RemoveTailBytes(byte[] pictureInBytes);
    }
}
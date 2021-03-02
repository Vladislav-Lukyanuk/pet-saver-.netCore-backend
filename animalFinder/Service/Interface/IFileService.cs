using System;
using System.Drawing;

namespace animalFinder.Service.Interface
{
    public interface IFileService
    {
        Guid Upload(string userId, byte[] file, string fileType, bool commit = true);
        Tuple<byte[], string> Get(Guid id);
        byte[] AsJpeg(Image img);
        byte[] AsJpeg(byte[] data);
        byte[] Resize(byte[] data, int width);
        byte[] Compress(byte[] data);
    }
}

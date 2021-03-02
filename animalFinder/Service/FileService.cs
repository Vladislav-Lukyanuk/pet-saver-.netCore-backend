using animalFinder.Service.Interface;
using DAL;
using DAL.Entity;
using DAL.Provider.Interface;
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace animalFinder.Service
{
    public class FileService: IFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileDataProvider _fileDataProvider;
        public FileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _fileDataProvider = unitOfWork.Get<IFileDataProvider>();
        }
        public Guid Upload(string userId, byte[] file, string fileType, bool commit = true)
        {
            var uploadedFile = _fileDataProvider.Add(new DAL.Entity.File()
            {
                FileBytes = file,
                Type = fileType,
                UserId = userId
            });

            if(commit)
            {
                _unitOfWork.Commit();
            }

            return uploadedFile.Id;
        }

        public Tuple<byte[], string> Get(Guid id)
        {
            var file = _fileDataProvider.Get(id);

            return new Tuple<byte[], string>(file.FileBytes, file.Type);
        }

        public byte[] AsJpeg(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }

        public byte[] AsJpeg(byte[] data)
        {
            using (var inStream = new MemoryStream(data))
            using (var outStream = new MemoryStream())
            {
                var imageStream = Image.FromStream(inStream);
                imageStream.Save(outStream, ImageFormat.Jpeg);
                return outStream.ToArray();
            }
        }

        public byte[] Resize(byte[] data, int width)
        {
            using (var stream = new MemoryStream(data))
            {
                var image = Image.FromStream(stream);

                var height = (width * image.Height) / image.Width;
                var thumbnail = image.GetThumbnailImage(width, height, null, IntPtr.Zero);

                using (var thumbnailStream = new MemoryStream())
                {
                    thumbnail.Save(thumbnailStream, ImageFormat.Jpeg);
                    return thumbnailStream.ToArray();
                }
            }
        }

        public byte[] Compress(byte[] data)
        {
            var jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            using (var inStream = new MemoryStream(data))
            using (var outStream = new MemoryStream())
            {
                var image = Image.FromStream(inStream);

                if (jpgEncoder == null)
                {
                    image.Save(outStream, ImageFormat.Jpeg);
                }
                else
                {
                    var qualityEncoder = Encoder.Quality;
                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(qualityEncoder, 75L);
                    image.Save(outStream, jpgEncoder, encoderParameters);
                }

                return outStream.ToArray();
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
    }
}

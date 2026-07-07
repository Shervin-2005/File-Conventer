using FileConverter.Application.Interfaces.ImageServiceInterfaces;
using FileConverter.Domain.Enums;
namespace FileConverter.Infrastracture.Images
{
    public class ImageInsparationService : IImageInsparationService
    {
        public bool GetImageFormatFromExtension(string extension, out ImageFormat initialFormat)
        {
            switch (extension)
            {
                case ".png":
                    initialFormat = ImageFormat.Png;
                    return true;

                case ".jpg":
                    initialFormat = ImageFormat.Jpg;
                    return true;

                case ".jpeg":
                    initialFormat = ImageFormat.Jpeg;
                    return true;

                case ".webp":
                    initialFormat = ImageFormat.Webp;
                    return true;

                case ".bmp":
                    initialFormat = ImageFormat.Bmp;
                    return true;

                case ".gif":
                    initialFormat = ImageFormat.Gif;
                    return true;

                case ".heif":
                    initialFormat = ImageFormat.Heif;
                    return true;

                default:
                    initialFormat = default;
                    return false;
            }
        }

        public bool GetImageTargetForamtFromEnum(ImageFormat targetFormat, out string targetExtension)
        {

            switch (targetFormat)
            {
                case ImageFormat.Png:
                    targetExtension = "png";
                    return true;

                case ImageFormat.Jpg:
                    targetExtension = "jpg";
                    return true;

                case ImageFormat.Jpeg:
                    targetExtension =  "jpeg";
                    return true;

                case ImageFormat.Webp:
                    targetExtension  = "webp";
                    return true;

                case ImageFormat.Bmp:
                    targetExtension =  "bmp";
                    return true;

                case ImageFormat.Gif:
                    targetExtension =  "gif";
                    return true;

                case ImageFormat.Heif:
                    targetExtension =  "heif";
                    return true;

                default:
                    targetExtension = default!;
                    return false;
            }
        }
    }
}

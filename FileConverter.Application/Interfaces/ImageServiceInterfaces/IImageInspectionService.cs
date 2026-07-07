using FileConverter.Domain.Enums;

namespace FileConverter.Application.Interfaces.ImageServiceInterfaces
{
    public interface IImageInspectionService
    {
        bool GetImageFormatFromExtension(string extension, out ImageFormat initialFormat);
        bool GetImageTargetForamtFromEnum(ImageFormat targetFormat, out string targetExtension);
    }
}

using FileConverter.Domain.Enums;

namespace FileConverter.Application.Interfaces.VideoServiceInterfaces
{
    public interface IVideoInspection
    {
        bool GetVideoFormatFromExtension(string extension, out VideoFormat initialFormat);
        bool GetVideoTargetForamtFromEnum(VideoFormat targetFormat, out string targetExtension);
    }
}

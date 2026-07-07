using FileConverter.Application.Interfaces.VideoServiceInterfaces;
using FileConverter.Domain.Enums;

namespace FileConverter.Infrastracture.Videos
{
    public class VideoInspectionService : IVideoInspection
    {
        public bool GetVideoFormatFromExtension(string extension, out VideoFormat initialFormat)
        {
            switch (extension)
            {
                case ".mp4":
                    initialFormat = VideoFormat.Mp4;
                    return true;

                case ".avi":
                    initialFormat = VideoFormat.Avi;
                    return true;

                case ".mov":
                    initialFormat = VideoFormat.Mov;
                    return true;

                case ".mkv":
                    initialFormat = VideoFormat.Mkv;
                    return true;

                case ".webm":
                    initialFormat = VideoFormat.Webm;
                    return true;

                default:
                    initialFormat = default;
                    return false;
            }
        }

        public bool GetVideoTargetForamtFromEnum(VideoFormat targetFormat, out string targetExtension)
        {
            switch (targetFormat)
            {
                case VideoFormat.Mp4:
                    targetExtension = "mp4";
                    return true;

                case VideoFormat.Avi:
                    targetExtension = "avi";
                    return true;

                case VideoFormat.Mov:
                    targetExtension = "mov";
                    return true;

                case VideoFormat.Mkv:
                    targetExtension = "mkv";
                    return true;

                case VideoFormat.Webm:
                    targetExtension = "webp";
                    return true;

                default:
                    targetExtension = default!;
                    return false;
            }
        }
    }
}

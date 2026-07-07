using FileConverter.Application.Interfaces.VideoServiceInterfaces;
using FileConverter.Application.UseCases.Interfaces.VideoInterfaces;
using FileConverter.Domain.Enums;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Implementations.VideosConversionsImplementations
{
    public class ConvertVideoUsecase : IConvertVideoUsecase
    {
        private readonly IConvertVideoService _convertVideoService;
        private readonly IVideoInspection _videoInspection;

        public ConvertVideoUsecase(IConvertVideoService convertVideoService,
            IVideoInspection videoInspection)
        {
            _convertVideoService = convertVideoService;
            _videoInspection = videoInspection;
        }

        public async Task<ConversionStreamResult> ExecuteAsync(
            Stream inputVideo,
            string originalFileName, 
            VideoFormat targetFormat,
            CancellationToken token)
        {
            double sizeMB = 0.0;
            if (inputVideo.CanSeek)
            {
                long size = inputVideo.Length;
                sizeMB = size / (1024.0 * 1024.0);
            }
            if (sizeMB == 0 || sizeMB > 150)
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    "حجم فایل نباید بیشتر از 150 مگابایت باشد.");
            }

            string extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (!_videoInspection.GetVideoFormatFromExtension(extension, out VideoFormat initialFormat))
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    $"فرمت فایل '{extension}' پشتیبانی نمی‌شود.");
            }

            if (initialFormat == targetFormat)
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    $"نمی توان دو فرمت یکسان را تبدیل کرد");
            }

            if (!_videoInspection.GetVideoTargetForamtFromEnum(targetFormat, out string targetExtension))
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    $"نمی توان به این فرمت تبدیل کرد: " + targetExtension);
            }

            return await _convertVideoService.ConvertStreamImageAsync(inputVideo, originalFileName, targetExtension, token);
        }
    }
}

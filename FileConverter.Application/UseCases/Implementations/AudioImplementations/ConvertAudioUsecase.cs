using FileConverter.Application.Interfaces.AudioServiceInterfaces;
using FileConverter.Application.UseCases.Interfaces.AudioInterfaces;
using FileConverter.Domain.Enums;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Implementations.AudioImplementations
{
    public class ConvertAudioUsecase : IConvertAudioUseCase
    {
        private readonly IConvertAudioService _convertAudioService;
        private readonly IAudioInspectionService _audioInspectionService;
        public ConvertAudioUsecase(IConvertAudioService convertAudioService, IAudioInspectionService audioInspectionService)
        {
            _convertAudioService = convertAudioService;
            _audioInspectionService = audioInspectionService;
        }

        public async Task<ConversionStreamResult> ExecuteAsync(
            Stream inputAudio,
            string originalFileName,
            AudioFormat targetFormat,
            CancellationToken token)
        {
            double sizeMB = 0.0;
            if (inputAudio.CanSeek)
            {
                long size = inputAudio.Length;
                sizeMB = size / (1024.0 * 1024.0);
            }
            if (sizeMB == 0 || sizeMB > 15)
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    "حجم فایل نباید بیشتر از 15 مگابایت باشد.");
            }

            string extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (!_audioInspectionService.GetAudioFormatFromExtension(extension, out AudioFormat initialFormat))
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    $"فرمت فایل '{extension}' پشتیبانی نمی‌شود.");
            }

            if (initialFormat == targetFormat)
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    $"نمی توان دو فرمت یکسان را تبدیل کرد");
            }

            if (!_audioInspectionService.GetAudioTargetForamtFromEnum(targetFormat, out string targetExtension))
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    $"نمی توان به این فرمت تبدیل کرد: " + targetExtension);
            }

            return await _convertAudioService.ConvertStreamAudioAsync(inputAudio, originalFileName, targetExtension, token);
        }
    }
}

using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.AudioServiceInterfaces
{
    public interface IConvertAudioService
    {
        Task<ConversionStreamResult> ConvertStreamAudioAsync(
            Stream inputImage,
            string originalFileName,
            string targetExtension,
            CancellationToken cancellationToken);
    }
}

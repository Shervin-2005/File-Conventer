using FileConverter.Domain.Enums;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Interfaces.AudioInterfaces
{
    public interface IConvertAudioUseCase
    {
        Task<ConversionStreamResult> ExecuteAsync(Stream inputAudio, string originalFileName,
       AudioFormat targetFormat,
       CancellationToken cancellationToken);
    }
}

using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.VideoServiceInterfaces
{
    public interface IConvertVideoService
    {
        Task<ConversionStreamResult> ConvertStreamImageAsync(
            Stream inputvideo,
            string originalFileName,
            string targetExtension,
            CancellationToken cancellationToken);
    }
}

using FileConverter.Domain.Enums;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Interfaces.VideoInterfaces
{
    public interface IConvertVideoUsecase
    {
        Task<ConversionStreamResult> ExecuteAsync(Stream inputVideo, string originalFileName,
       VideoFormat targetFormat,
       CancellationToken cancellationToken);
    }
}

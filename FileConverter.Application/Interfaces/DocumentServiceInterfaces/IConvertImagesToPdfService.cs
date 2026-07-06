using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IConvertImagesToPdfService
    {
        Task<ConversionStreamResult> ConvertImagesToPdfAsync(
           List<Stream> pdfs,
           List<string> fileNames,
           CancellationToken cancellationToken);
    }
}

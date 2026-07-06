using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IConvertDocxToTextService
    {
        Task<ConversionStreamResult> ConvertDocxToTextAsync(
            Stream inputDocument,
            string originalFileName,
            CancellationToken cancellationToken);
    }
}

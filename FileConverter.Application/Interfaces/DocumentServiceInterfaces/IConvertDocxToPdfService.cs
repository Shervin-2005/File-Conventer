using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IConvertDocxToPdfService
    {
        Task<ConversionStreamResult> ConvertDocxToPdfAsync(
            Stream inputDocument,
            string originalFileName,
            CancellationToken cancellationToken);
    }
}

using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IDecryptPdfService
    {
        Task<ConversionStreamResult> DecryptPdfAsync(
             Stream inputDocument,
             string originalFileName,
             string password,
             CancellationToken cancellationToken);
    }
}

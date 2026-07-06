using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IEncryptPdfService
    {
        Task<ConversionStreamResult> EncryptPdfAsync(
               Stream inputDocument,
               string originalFileName,
               string password,
               CancellationToken cancellationToken);
    }
}

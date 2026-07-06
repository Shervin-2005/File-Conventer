using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces
{
    public interface IEncryptDocumentUsecase
    {
        Task<ConversionStreamResult> ExecuteAsync(Stream inputDocument, string originalFileName,
        string password,
        CancellationToken cancellationToken);
    }
}

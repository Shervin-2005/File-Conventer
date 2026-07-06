using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IConvertTxtToPdfService
    {
        Task<ConversionStreamResult> ConvertTxtToPdfAsync(
            Stream inputDocument,
            string originalFileName,
            CancellationToken cancellationToken);
    }
}

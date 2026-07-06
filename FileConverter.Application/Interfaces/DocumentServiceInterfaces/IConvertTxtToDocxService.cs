using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IConvertTxtToDocxService
    {
        Task<ConversionStreamResult> ConvertTxtToDocxAsync(
            Stream inputDocument,
            string originalFileName,
            CancellationToken cancellationToken);
    }
}

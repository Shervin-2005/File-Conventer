using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IConvertPdfToTxtService
    {
        Task<ConversionStreamResult> ConvertPdfToTxtAsync(
            Stream inputDocument,
            string originalFileName,
            int? startPage,
            int? endPage,
            CancellationToken cancellationToken);
    }
}

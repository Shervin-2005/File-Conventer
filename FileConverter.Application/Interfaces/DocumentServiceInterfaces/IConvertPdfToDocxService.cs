using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IConvertPdfToDocxService
    {
        Task<ConversionStreamResult> ConvertPdfToDocxAsync(
            Stream inputDocument,
            string originalFileName,
            int? startPage,
            int? endPage,
            CancellationToken cancellationToken);    
    }
}

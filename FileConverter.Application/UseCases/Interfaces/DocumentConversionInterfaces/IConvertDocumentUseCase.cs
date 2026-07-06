using FileConverter.Domain.Enums;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces
{
    public interface IConvertDocumentUseCase
    {
        Task<ConversionStreamResult> ExecuteAsync(Stream inputDocument, string originalFileName,
        DocumentFormat targetFormat,
        int? startPage,
        int? endPage,
        CancellationToken cancellationToken);
    }
}

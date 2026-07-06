using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces
{
    public interface IConvertImageToPdfUsecase
    {
        Task<ConversionStreamResult> ExecuteAsync(List<Stream> streams, List<string> fileNames,
           CancellationToken cancellationToken);
    }
}

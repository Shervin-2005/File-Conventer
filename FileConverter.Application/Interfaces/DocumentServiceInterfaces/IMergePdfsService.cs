using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.DocumentServiceInterfaces
{
    public interface IMergePdfsService
    {
        Task<ConversionStreamResult> MergePdfsAsync(
           List<Stream> pdfs,
           List<string> fileNames,
           CancellationToken cancellationToken);
    }
}

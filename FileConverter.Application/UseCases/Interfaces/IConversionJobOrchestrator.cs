using FileConverter.Domain.Entities;

namespace FileConverter.Application.UseCases.Interfaces
{
    public interface IConversionJobOrchestrator
    {
        Task<ConversionJob?> StartJobAsync(
            int userId, Stream fileStream, string fileName,
            string contentType, string targetFormat, CancellationToken token);

        Task<string?> CompleteJobAsync(
            ConversionJob? job, int userId, Stream outputStream,
            string outputFileName, string contentType, CancellationToken token);

        Task FailJobAsync(ConversionJob? job, string errorMessage, CancellationToken token);
    }
}
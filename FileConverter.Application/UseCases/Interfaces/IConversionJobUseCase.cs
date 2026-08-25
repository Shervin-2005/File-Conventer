using FileConverter.Application.DTOs;
using FileConverter.Domain.Entities;
using FileConverter.Domain.Enums;

namespace FileConverter.Application.UseCases.Interfaces
{
    public interface IConversionJobUseCase
    {
        Task<ConversionJob> CreatePendingJobAsync(int userId, string sourceFormat, string targetFormat,
            MediaCategory sourceCategory, MediaCategory targetCategory, CancellationToken token);

        Task SetOriginalFileUrlAsync(int jobId, string originalFileUrl, CancellationToken token);
        Task MarkProcessingAsync(int jobId, CancellationToken token);
        Task MarkCompletedAsync(int jobId, string outputFileUrl, CancellationToken token);
        Task MarkFailedAsync(int jobId, string errorMessage, CancellationToken token);
    }
}

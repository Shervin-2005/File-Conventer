using FileConverter.Application.Interfaces;
using FileConverter.Application.UseCases.Interfaces;
using FileConverter.Domain.Entities;
using FileConverter.Domain.Enums;

namespace FileConverter.Application.UseCases.Implementations
{
    public class ConversionJobUseCase : IConversionJobUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConversionJobUseCase(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<ConversionJob> CreatePendingJobAsync(int userId, string sourceFormat, string targetFormat,
            MediaCategory sourceCategory, MediaCategory targetCategory, CancellationToken token)
        {
            var job = new ConversionJob
            {
                UserId = userId,
                SourceFormat = sourceFormat,
                TargetFormat = targetFormat,
                SourceCategory = sourceCategory,
                TargetCategory = targetCategory,
                Status = ConversionJob.ConversionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ConversionJobs.AddAsync(job);
            await _unitOfWork.SaveChangesAsync(); 
            return job;
        }

        public async Task SetOriginalFileUrlAsync(int jobId, string originalFileUrl, CancellationToken token)
        {
            var job = await _unitOfWork.ConversionJobs.GetByIdAsync(jobId);
            if (job is null) return;

            job.OriginalFileUrl = originalFileUrl;
            _unitOfWork.ConversionJobs.Update(job);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkProcessingAsync(int jobId, CancellationToken token)
        {
            var job = await _unitOfWork.ConversionJobs.GetByIdAsync(jobId);
            if (job is null) return;

            job.Status = ConversionJob.ConversionStatus.Processing;
            _unitOfWork.ConversionJobs.Update(job);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkCompletedAsync(int jobId, string outputFileUrl, CancellationToken token)
        {
            var job = await _unitOfWork.ConversionJobs.GetByIdAsync(jobId);
            if (job is null) return;

            job.Status = ConversionJob.ConversionStatus.Completed;
            job.OutputFileUrl = outputFileUrl;
            job.CompletedAt = DateTime.UtcNow;
            _unitOfWork.ConversionJobs.Update(job);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkFailedAsync(int jobId, string errorMessage, CancellationToken token)
        {
            var job = await _unitOfWork.ConversionJobs.GetByIdAsync(jobId);
            if (job is null) return;

            job.Status = ConversionJob.ConversionStatus.Failed;
            job.ErrorMessage = errorMessage;
            job.CompletedAt = DateTime.UtcNow;
            _unitOfWork.ConversionJobs.Update(job);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

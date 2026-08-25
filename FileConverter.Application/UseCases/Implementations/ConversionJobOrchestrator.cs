using FileConverter.Application.Common;
using FileConverter.Application.Interfaces;
using FileConverter.Application.UseCases.Interfaces;
using FileConverter.Domain.Entities;

namespace FileConverter.Application.UseCases.Implementations
{
    public class ConversionJobOrchestrator : IConversionJobOrchestrator
    {
        private readonly IConversionJobUseCase _conversionJobUseCase;
        private readonly IImageStorageService _storageService;

        public ConversionJobOrchestrator(
            IConversionJobUseCase conversionJobUseCase,
            IImageStorageService storageService)
        {
            _conversionJobUseCase = conversionJobUseCase;
            _storageService = storageService;
        }

        public async Task<ConversionJob?> StartJobAsync(
            int userId, Stream fileStream, string fileName,
            string contentType, string targetFormat, CancellationToken token)
        {
            if (userId == 0) return null;

            var sourceCategory = MediaCategoryHelper.GetCategory(fileName);
            var targetCategory = MediaCategoryHelper.GetCategory("." + targetFormat);

            var job = await _conversionJobUseCase.CreatePendingJobAsync(
                userId,
                sourceFormat: Path.GetExtension(fileName).TrimStart('.'),
                targetFormat: targetFormat,
                sourceCategory: sourceCategory,
                targetCategory: targetCategory,
                token);

            var originalUrl = await _storageService.UploadAsync(
                fileStream,
                folder: $"conversions/{userId}/{job.Id}/originalFile",
                fileName: fileName,
                contentType: contentType);

            await _conversionJobUseCase.SetOriginalFileUrlAsync(job.Id, originalUrl, token);
            await _conversionJobUseCase.MarkProcessingAsync(job.Id, token);

            return job;
        }

        public async Task<string?> CompleteJobAsync(
            ConversionJob? job, int userId, Stream outputStream,
            string outputFileName, string contentType, CancellationToken token)
        {
            if (job is null) return null;

            outputStream.Position = 0;

            var outputUrl = await _storageService.UploadAsync(
                outputStream,
                folder: $"conversions/{userId}/{job.Id}/converted",
                fileName: outputFileName,
                contentType: contentType);

            await _conversionJobUseCase.MarkCompletedAsync(job.Id, outputUrl, token);

            return outputUrl;
        }

        public async Task FailJobAsync(ConversionJob? job, string errorMessage, CancellationToken token)
        {
            if (job is null) return;
            await _conversionJobUseCase.MarkFailedAsync(job.Id, errorMessage, token);
        }
    }
}
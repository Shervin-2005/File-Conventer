using FileConverter.Domain.Enums;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Interfaces.ImagesConversionsInterfaces;

public interface IConvertImageUseCase
{
    Task<ConversionStreamResult> ExecuteAsync(Stream inputImage, string originalFileName,
        ImageFormat targetFormat,
        CancellationToken cancellationToken);
}
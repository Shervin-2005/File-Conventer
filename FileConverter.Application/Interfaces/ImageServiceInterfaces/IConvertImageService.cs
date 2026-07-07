using FileConverter.Domain.Models;

namespace FileConverter.Application.Interfaces.ImageServiceInterfaces;

public interface IConvertImageSevice
{
    Task<ConversionStreamResult> ConvertStreamImageAsync(
        Stream inputImage,
        string originalFileName,
        string targetExtension,
        CancellationToken cancellationToken);
}
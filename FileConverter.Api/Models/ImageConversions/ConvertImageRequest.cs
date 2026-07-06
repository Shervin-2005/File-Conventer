using FileConverter.Domain.Enums;

namespace FileConverter.Api.Models.ImageConversions;

public class ConvertImageRequest
{
    public IFormFile File { get; set; } = null!;
    public ImageFormat TargetFormat { get; set; }
}
using FileConverter.Application.Interfaces.ImageServiceInterfaces;
using FileConverter.Application.UseCases.Interfaces.ImagesConversionsInterfaces;
using FileConverter.Domain.Enums;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Implementations.ImagesConversionImplementations;

public class ConvertImageUseCase : IConvertImageUseCase
{
    private readonly IConvertImageSevice _convertImageSevice;
    private readonly IImageInspectionService _imageInsparationService;

    public ConvertImageUseCase(IConvertImageSevice convertImageSevice, IImageInspectionService imageInsparationService)
    {
        _convertImageSevice = convertImageSevice;
        _imageInsparationService = imageInsparationService;
    }

    public async Task<ConversionStreamResult> ExecuteAsync(Stream inputImage, string originalFileName,
        ImageFormat targetFormat,
        CancellationToken token)
    {
        double sizeMB = 0.0;
        if (inputImage.CanSeek)
        {
            long size = inputImage.Length;
            sizeMB = size / (1024.0 * 1024.0);
        }
        if (sizeMB == 0 || sizeMB > 15)
        {
            return new ConversionStreamResult(null, null, string.Empty, 0, false,
                "حجم فایل نباید بیشتر از 15 مگابایت باشد.");
        }
        
        string extension = Path.GetExtension(originalFileName).ToLowerInvariant();

        if(!_imageInsparationService.GetImageFormatFromExtension(extension,out ImageFormat initialFormat))
        {
            return new ConversionStreamResult(null, null, string.Empty, 0, false,
                $"فرمت فایل '{extension}' پشتیبانی نمی‌شود.");
        }

        if(initialFormat == targetFormat)
        {
            return new ConversionStreamResult(null, null, string.Empty, 0, false,
                $"نمی توان دو فرمت یکسان را تبدیل کرد");
        }

        if(!_imageInsparationService.GetImageTargetForamtFromEnum(targetFormat, out string targetExtension))
        {
            return new ConversionStreamResult(null, null, string.Empty, 0, false,
                $"نمی توان به این فرمت تبدیل کرد: " + targetExtension);
        }

        return await _convertImageSevice.ConvertStreamImageAsync(inputImage, originalFileName, targetExtension, token);
    }
}
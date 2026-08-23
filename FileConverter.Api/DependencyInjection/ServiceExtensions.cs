using FileConverter.Application.Interfaces;
using FileConverter.Application.Interfaces.AudioServiceInterfaces;
using FileConverter.Application.Interfaces.AuthInterfaces;
using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Application.Interfaces.ImageServiceInterfaces;
using FileConverter.Application.Interfaces.VideoServiceInterfaces;
using FileConverter.Application.UseCases.Implementations.AudioImplementations;
using FileConverter.Application.UseCases.Implementations.AuthImplementations;
using FileConverter.Application.UseCases.Implementations.DocumentConversionImplementations;
using FileConverter.Application.UseCases.Implementations.ImagesConversionImplementations;
using FileConverter.Application.UseCases.Implementations.VideosConversionsImplementations;
using FileConverter.Application.UseCases.Interfaces.AudioInterfaces;
using FileConverter.Application.UseCases.Interfaces.AuthInterfaces;
using FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces;
using FileConverter.Application.UseCases.Interfaces.ImagesConversionsInterfaces;
using FileConverter.Application.UseCases.Interfaces.VideoInterfaces;
using FileConverter.Infrastracture.Services.Audios;
using FileConverter.Infrastracture.Services.Document;
using FileConverter.Infrastracture.Services.Images;
using FileConverter.Infrastracture.Services.Videos;
using Infrastructure.Services;

namespace FileConverter.Api.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        //documents
        services.AddScoped<IConvertPdfToDocxService, PdfToDocxConversionService>();
        services.AddScoped<IConvertDocxToPdfService, DocxToPdfConversionService>();
        services.AddScoped<IConvertPdfToTxtService, PdfToTxtConversionService>();
        services.AddScoped<IConvertTxtToDocxService, TxtToDocxConversionService>();
        services.AddScoped<IConvertTxtToPdfService, TxtToPdfConversionService>();
        services.AddScoped<IConvertDocxToTextService, DocxToTextConversionService>();
        services.AddScoped<IMergePdfsService, MergePdfsService>();
        services.AddScoped<IEncryptPdfService, EncryptPdfService>();
        services.AddScoped<IDecryptPdfService, DecryptPdfService>();
        services.AddScoped<IConvertImagesToPdfService, ImagesToPdfService>();
        services.AddScoped<IPdfInspectionService, PdfInspectionService>();

        //images
        services.AddScoped<IConvertImageSevice, ConvertImageSevice>();
        services.AddScoped<IImageInspectionService, ImageInspectionService>();

        //Audios
        services.AddScoped<IConvertAudioService, ConvertAudioService>();
        services.AddScoped<IAudioInspectionService, AudioInspectionService>();

        //videos
        services.AddScoped<IConvertVideoService, ConvertVideoService>();
        services.AddScoped<IVideoInspection, VideoInspectionService>();

        //Auth
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<ISmsService,  SmsService>();
        services.AddScoped<ITokenService, TokenService>();

        //User
        services.AddScoped<IImageStorageService, S3ImageStorageService>();

        return services;
    }

    public static IServiceCollection AddUseCase(this IServiceCollection services)
    {
        //documents
        services.AddScoped<IConvertDocumentUseCase, ConvertDocumentUseCase>();
        services.AddScoped<IMergeDocumentUsecase, MergeDocumentUseCase>();
        services.AddScoped<IEncryptDocumentUsecase, EncryptDocumentUseCase>();
        services.AddScoped<IDecryptDocumentUsecase, DecryptDocumentUsecase>();
        services.AddScoped<IConvertImageToPdfUsecase, ConvertImageToPdfUsecase>();

        //images
        services.AddScoped<IConvertImageUseCase, ConvertImageUseCase>();

        //Audios
        services.AddScoped<IConvertAudioUseCase, ConvertAudioUsecase>();

        //videos
        services.AddScoped<IConvertVideoUsecase, ConvertVideoUsecase>();

        //Auth
        services.AddScoped<IAuthUseCase, AuthUseCase>();
        services.AddScoped<IOtpUseCase, OtpUseCase>();
        services.AddScoped<IUserUseCase, UserUseCase>();

        return services;
    }
}
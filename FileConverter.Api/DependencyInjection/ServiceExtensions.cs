using FileConverter.Application.Interfaces.ImageServiceInterfaces;
using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Infrastracture.Document;
using FileConverter.Application.UseCases.Implementations.DocumentConversionImplementations;
using FileConverter.Application.UseCases.Implementations.ImagesConversionImplementations;
using FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces;
using FileConverter.Application.UseCases.Interfaces.ImagesConversionsInterfaces;
using FileConverter.Infrastracture.Images;

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

        //imagess
        services.AddScoped<IConvertImageUseCase, ConvertImageUseCase>();
        services.AddScoped<IImageInsparationService, ImageInsparationService>();

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
        services.AddScoped<IConvertImageSevice, ConvertImageSevice>();

        return services;
    }
}
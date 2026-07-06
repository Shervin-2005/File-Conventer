using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Implementations.DocumentConversionImplementations
{
    public class ConvertImageToPdfUsecase : IConvertImageToPdfUsecase
    {
        private readonly IConvertImagesToPdfService _convertImagesToPdfService;

        public ConvertImageToPdfUsecase(IConvertImagesToPdfService convertImagesToPdfService) 
        {
            _convertImagesToPdfService = convertImagesToPdfService;
        }
        public async Task<ConversionStreamResult> ExecuteAsync(
            List<Stream> streams,
            List<string> fileNames,
            CancellationToken cancellationToken)
        {
            if (streams.Count > 50)
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                           "نمی توان بیشتر از 50 عکس انتخاب کرد");
            }

            double sizeMB = 0.0;
            foreach (Stream stream in streams)
            {
                if (stream.CanSeek)
                {
                    long size = stream.Length;
                    sizeMB = size / (1024.0 * 1024.0);
                }

                if (sizeMB == 0 || sizeMB > 15)
                {
                    return new ConversionStreamResult(null, null, string.Empty, 0, false,
                        "حجم عکس نباید بیشتر از 15 مگابایت باشد.");
                }
            }
            foreach (string name in fileNames)
            {
                string extension = Path.GetExtension(name).ToLowerInvariant();
                if (extension != ".png" && extension!= ".jpeg" && extension != ".jpg")
                {
                    return new ConversionStreamResult(null, null, string.Empty, 0, false,
                        "فایل مورد نظر عکس نمیباشد");
                }
            }
            return await _convertImagesToPdfService.
                           ConvertImagesToPdfAsync(streams, fileNames, cancellationToken);
        }
    }
}

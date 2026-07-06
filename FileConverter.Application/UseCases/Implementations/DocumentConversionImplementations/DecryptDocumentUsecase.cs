using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Implementations.DocumentConversionImplementations
{
    public class DecryptDocumentUsecase : IDecryptDocumentUsecase
    {
        private readonly IDecryptPdfService _decryptPdfService;

        public DecryptDocumentUsecase(IDecryptPdfService decryptPdfService)
        {
           _decryptPdfService = decryptPdfService;
        }

        public async Task<ConversionStreamResult> ExecuteAsync(Stream inputDocument, string originalFileName, string password, CancellationToken cancellationToken)
        {
            double sizeMB = 0.0;

            if (inputDocument.CanSeek)
            {
                long size = inputDocument.Length;
                sizeMB = size / (1024.0 * 1024.0);
            }

            if (sizeMB == 0 || sizeMB > 100)
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    "حجم فایل نباید بیشتر از 100 مگابایت باشد.");
            }

            string extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (extension != ".pdf")
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    "فرمت اولیه نامعتبر است");
            }

            return await _decryptPdfService.
                DecryptPdfAsync(inputDocument, originalFileName, password, cancellationToken);
        }
    }
}

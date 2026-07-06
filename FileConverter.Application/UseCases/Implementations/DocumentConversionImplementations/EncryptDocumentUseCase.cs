using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Implementations.DocumentConversionImplementations
{
    public class EncryptDocumentUseCase : IEncryptDocumentUsecase
    {
        private readonly IEncryptPdfService _encryptPdfService;
        private readonly IPdfInspectionService _pdfInspectionService;
        public EncryptDocumentUseCase(IEncryptPdfService encryptPdfService, IPdfInspectionService pdfInspectionService)
        {
            _encryptPdfService = encryptPdfService;
            _pdfInspectionService = pdfInspectionService;
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

            if(extension != ".pdf")
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    "فرمت اولیه نامعتبر است");
            }

            bool isEncrypted = await _pdfInspectionService.IsPdfEncryptedAsync(inputDocument, cancellationToken);
            if (isEncrypted)
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    $"فایل '{originalFileName}' رمزگذاری شده است. لطفاً ابتدا رمز آن را حذف کنید.");
            }

            return await _encryptPdfService.
                EncryptPdfAsync(inputDocument,originalFileName, password, cancellationToken);
            
        }
    }
}

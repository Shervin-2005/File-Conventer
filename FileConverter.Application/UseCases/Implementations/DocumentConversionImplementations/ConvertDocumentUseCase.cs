using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces;
using FileConverter.Domain.Enums;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Implementations.DocumentConversionImplementations
{
    public class ConvertDocumentUseCase : IConvertDocumentUseCase
    {
        private readonly IConvertPdfToDocxService _pdf2DocxService;
        private readonly IConvertDocxToPdfService _docx2PdfService;
        private readonly IConvertPdfToTxtService _pdf2TxtService;
        private readonly IConvertTxtToDocxService _convertTxtToDocx;
        private readonly IConvertTxtToPdfService _convertTxtToPdfService;
        private readonly IConvertDocxToTextService _convertDocxToTextService;
        private readonly IPdfInspectionService _pdfInspectionService;

        public ConvertDocumentUseCase(IConvertPdfToDocxService pdf2DocxService,
                                      IConvertDocxToPdfService docx2PdfService,
                                      IConvertPdfToTxtService pdf2TxtService,
                                      IConvertTxtToDocxService convertTxtToDocx,
                                      IConvertTxtToPdfService convertTxtToPdfService,
                                      IConvertDocxToTextService convertDocxToTextService,
                                      IPdfInspectionService pdfInspectionService)
        {
            _pdf2DocxService = pdf2DocxService;
            _docx2PdfService = docx2PdfService;
            _pdf2TxtService = pdf2TxtService;
            _convertTxtToDocx = convertTxtToDocx;
            _convertTxtToPdfService = convertTxtToPdfService;
            _convertDocxToTextService = convertDocxToTextService;
            _pdfInspectionService = pdfInspectionService;
        }

        public async Task<ConversionStreamResult> ExecuteAsync(Stream inputDocument, string originalFileName,
            DocumentFormat targetFormat,
            int? startPage,
            int? endPage,
            CancellationToken cancellationToken)
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

            if (startPage <= 0 || endPage <= 0)
            {
                startPage = 1;
                endPage = int.MaxValue;
                
            }
            else if(startPage > endPage)
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    "صفحه شروع نمی‌تواند بزرگ‌تر از صفحه پایان باشد.");
            }

            string extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (extension == ".pdf")
            {
               
                bool isEncrypted = await _pdfInspectionService.IsPdfEncryptedAsync(inputDocument, cancellationToken);
                if (isEncrypted)
                {
                    return new ConversionStreamResult(null, null, string.Empty, 0, false,
                        $"فایل '{originalFileName}' رمزگذاری شده است. لطفاً ابتدا رمز آن را حذف کنید.");
                }
                
                switch (targetFormat)
                {
                    case DocumentFormat.Pdf:
                        return new ConversionStreamResult(null, null, string.Empty, 0, false,
                                                          "نمیشه pdf را به pdf تبدیل کرد");
                    case DocumentFormat.Docx:
                        return await _pdf2DocxService.
                             ConvertPdfToDocxAsync(inputDocument, originalFileName, startPage, endPage, cancellationToken);
                    case DocumentFormat.Txt:
                        return await _pdf2TxtService.
                            ConvertPdfToTxtAsync(inputDocument, originalFileName, startPage, endPage, cancellationToken);
                    default:
                        return new ConversionStreamResult(null, null, string.Empty, 0, false,
                               "نمیشه به این فرمت تبدیل کرد");
                }
            }
            else if (extension == ".docx")
            {
                switch (targetFormat)
                {
                    case DocumentFormat.Docx:
                        return new ConversionStreamResult(null, null, string.Empty, 0, false,
                                                          "نمیشه docx را به docx تبدیل کرد");
                    case DocumentFormat.Pdf:
                        return await _docx2PdfService.
                             ConvertDocxToPdfAsync(inputDocument, originalFileName, cancellationToken);

                    case DocumentFormat.Txt:
                        return await _convertDocxToTextService.
                            ConvertDocxToTextAsync(inputDocument, originalFileName, cancellationToken);

                    default:
                        return new ConversionStreamResult(null, null, string.Empty, 0, false,
                                                           "نمیشه به این فرمت تبدیل کرد");
                }
            }
            else if (extension == ".txt")
            {
                switch (targetFormat)
                {
                    case DocumentFormat.Txt:
                        return new ConversionStreamResult(null, null, string.Empty, 0, false,
                                                          "نمیشه txt را به txt تبدیل کرد");
                    case DocumentFormat.Docx:
                        return await _convertTxtToDocx.
                             ConvertTxtToDocxAsync(inputDocument, originalFileName, cancellationToken);

                    case DocumentFormat.Pdf:
                        return await _convertTxtToPdfService.
                            ConvertTxtToPdfAsync(inputDocument, originalFileName, cancellationToken);
                    default:
                        return new ConversionStreamResult(null, null, string.Empty, 0, false,
                                                           "نمیشه به این فرمت تبدیل کرد");
                }
            }
            else
            {
                return new ConversionStreamResult(null, null, string.Empty, 0, false,
                    "فرمت اولیه نامعتبر است");
            }
        }
    }
}
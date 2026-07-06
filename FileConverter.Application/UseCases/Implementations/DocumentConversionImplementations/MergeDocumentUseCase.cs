using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces;
using FileConverter.Domain.Models;

namespace FileConverter.Application.UseCases.Implementations.DocumentConversionImplementations
{
    public class MergeDocumentUseCase : IMergeDocumentUsecase
    {
        private readonly IMergePdfsService _mergePdfsService;
        private readonly IPdfInspectionService _pdfInspectionService;

        public MergeDocumentUseCase(IMergePdfsService mergePdfsService, IPdfInspectionService pdfInspectionService)
        {
            _mergePdfsService = mergePdfsService;
            _pdfInspectionService = pdfInspectionService;
        }
        public async Task<ConversionStreamResult> ExecuteAsync(List<Stream> streams, List<string> fileNames, CancellationToken cancellationToken)
        {
            double sizeMB = 0.0;
            foreach (Stream stream in streams)
            {
                if (stream.CanSeek)
                {
                    long size = stream.Length;
                    sizeMB = size / (1024.0 * 1024.0);
                }

                if (sizeMB == 0 || sizeMB > 100)
                {
                    return new ConversionStreamResult(null, null, string.Empty, 0, false,
                        "حجم فایل نباید بیشتر از 100 مگابایت باشد.");
                }
            }
            foreach(string name in fileNames)
            {
                string extension = Path.GetExtension(name).ToLowerInvariant();
                if(extension != ".pdf")
                {
                        return new ConversionStreamResult(null, null, string.Empty, 0, false,
                            "فایل مورد نظر pdf نمیباشد " + name );
                }
                // Check each stream for encryption BEFORE saving anything to disk
                for (int i = 0; i < streams.Count; i++)
                {
                    bool isEncrypted = await _pdfInspectionService.IsPdfEncryptedAsync(streams[i], cancellationToken);
                    if (isEncrypted)
                    {
                        return new ConversionStreamResult(null, null, string.Empty, 0, false,
                            $"فایل '{fileNames[i]}' رمزگذاری شده است. لطفاً ابتدا رمز آن را حذف کنید.");
                    }
                }
            }
            return await _mergePdfsService.
                            MergePdfsAsync(streams,fileNames, cancellationToken);
        }
    }
}


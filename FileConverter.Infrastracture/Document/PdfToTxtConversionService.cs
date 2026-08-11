using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Domain.Models;
using Python.Runtime;

namespace FileConverter.Infrastracture.Document
{
    public class PdfToTxtConversionService : IConvertPdfToTxtService
    {
        public async Task<ConversionStreamResult> ConvertPdfToTxtAsync(
            Stream inputDocument,
            string originalFileName,
            int? startPage, 
            int? endPage,
            CancellationToken cancellationToken)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            string pdfFilePath = Path.Combine(tempFolder, originalFileName);
            string outputFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".txt";
            string outputPath = Path.Combine(tempFolder, outputFileName);

            try
            {
                // Save the incoming stream to a real file on disk (Python needs a file path)
                using (var fileStream = new FileStream(pdfFilePath, FileMode.Create, FileAccess.Write))
                {
                    await inputDocument.CopyToAsync(fileStream, cancellationToken);
                }

                await Task.Run(() => RunPyScript(pdfFilePath, outputPath, (startPage - 1).ToString()!, endPage.ToString()!), cancellationToken);

                // Read the converted .txt back into memory so we can return it
                var resultStream = new MemoryStream();
                using (var convertedFile = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                {
                    await convertedFile.CopyToAsync(resultStream, cancellationToken);
                }
                resultStream.Position = 0;

                return new ConversionStreamResult(
                    OutputStream: resultStream,
                    FileName: outputFileName,
                    ContentType: "text/plain",
                    SizeInBytes: resultStream.Length,
                    Success: true,
                    ErrorMessage: null
                    );
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return new ConversionStreamResult(
                    OutputStream: null,
                    FileName: null,
                    ContentType: "application/octet-stream",
                    SizeInBytes: 0,
                    Success: false,
                    ErrorMessage: ex.Message
                );
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, recursive: true);
                }
            }
        }
        
        private static void RunPyScript(string pdfFilePath, string outputPath, string startPage, string endPage)
        {
            // Python engine is already initialized once at app startup (Program.cs).
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"D:\Project\C#\File-Converter-Backend\File-Conventer\FileConverter.Infrastracture\Document\PythonScripts");

                var pdf = new PyString(pdfFilePath);
                var output = new PyString(outputPath);
                var start  = new PyString(startPage);
                var end = new PyString(endPage);

                var pythonScript = Py.Import("PdfToTxt");
                
                // پارامترهای صفحه‌بندی به اسکریپت پایتون اضافه شدند
                pythonScript.InvokeMethod("ConvertPdfToTxt", new PyObject[] { pdf, output, start, end });
            }
        }
    }
}
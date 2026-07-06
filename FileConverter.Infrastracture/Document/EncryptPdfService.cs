using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Domain.Models;
using Python.Runtime;

namespace FileConverter.Infrastracture.Document
{
    public class EncryptPdfService : IEncryptPdfService
    {
        public async Task<ConversionStreamResult> EncryptPdfAsync(
            Stream inputDocument,
            string originalFileName,
            string password,
            CancellationToken cancellationToken)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            string pdfFilePath = Path.Combine(tempFolder, originalFileName);
            string outputFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".pdf";
            string outputPath = Path.Combine(tempFolder, outputFileName);

            try
            {
                using (var fileStream = new FileStream(pdfFilePath, FileMode.Create, FileAccess.Write))
                {
                    await inputDocument.CopyToAsync(fileStream, cancellationToken);
                }

                await Task.Run(() => RunPyScript(pdfFilePath, outputPath ,password), cancellationToken);

                // 4. Read the converted .pdf back into memory so we can return it
                var resultStream = new MemoryStream();
                using (var convertedFile = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                {
                    await convertedFile.CopyToAsync(resultStream, cancellationToken);
                }
                resultStream.Position = 0;

                return new ConversionStreamResult(
                    OutputStream: resultStream,
                    FileName: outputFileName,
                    ContentType: "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    SizeInBytes: resultStream.Length,
                    Success: true,
                    ErrorMessage: null
                    );
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
        private static void RunPyScript(string pdfFilePath, string outputPath, string password)
        {
            if (!PythonEngine.IsInitialized)
            {
                Runtime.PythonDLL = @"C:\Users\CLINIC\AppData\Local\Programs\Python\Python314\python314.dll";
                PythonEngine.Initialize();
            }

            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"E:\Project\File Converter\FileConverter.Infrastracture\Document\PythonScripts");

                var pdf = new PyString(pdfFilePath);
                var output = new PyString(outputPath);
                var pdfPassword = new PyString(password);

                var pythonScript = Py.Import("EncryptDecryptPdf");
                pythonScript.InvokeMethod("EncryptPdf", new PyObject[] { pdf, output, pdfPassword });
            }
        }
    }
}

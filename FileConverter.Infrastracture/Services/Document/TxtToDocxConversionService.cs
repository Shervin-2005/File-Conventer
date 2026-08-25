using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Domain.Models;
using Python.Runtime;

namespace FileConverter.Infrastracture.Services.Document
{
    public class TxtToDocxConversionService : IConvertTxtToDocxService
    {
        public async Task<ConversionStreamResult> ConvertTxtToDocxAsync(
            Stream inputDocument, 
            string originalFileName, 
            CancellationToken cancellationToken)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            string txtFilePath = Path.Combine(tempFolder, originalFileName);
            string outputFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".docx";
            string outputPath = Path.Combine(tempFolder, outputFileName);

            try
            {
                // 2. Save the incoming stream to a real file on disk (Python needs a file path)
                using (var fileStream = new FileStream(txtFilePath, FileMode.Create, FileAccess.Write))
                {
                    await inputDocument.CopyToAsync(fileStream, cancellationToken);
                }

                await Task.Run(() => RunPyScript(txtFilePath, outputPath), cancellationToken);

                // 4. Read the converted .docx back into memory so we can return it
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
        private static void RunPyScript(string txtFilePath, string outputPath)
        {
            if (!PythonEngine.IsInitialized)
            {
                Runtime.PythonDLL = @"C:\Users\CLINIC\AppData\Local\Programs\Python\Python314\python314.dll";
                PythonEngine.Initialize();
            }

            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"E:\Project\File Converter\FileConverter.Infrastracture\Services\Document\PythonScripts");

                var txt = new PyString(txtFilePath);
                var output = new PyString(outputPath);

                var pythonScript = Py.Import("TxtConversions");
                pythonScript.InvokeMethod("ConvertTxtToDocx", new PyObject[] { txt, output });
            }
        }
    }
}

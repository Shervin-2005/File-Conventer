using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Domain.Models;
using Python.Runtime;

namespace FileConverter.Infrastracture.Document
{
    public class ImagesToPdfService : IConvertImagesToPdfService
    {
       public async Task<ConversionStreamResult> ConvertImagesToPdfAsync(
            List<Stream> pdfs,
            List<string> fileNames,
            CancellationToken cancellationToken)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            var savedPaths = new List<string>();
            string outputFileName = BuildOutputFileName(fileNames);
            string outputPath = Path.Combine(tempFolder, outputFileName);

            try
            {
                // 1. Save every incoming stream to a real file on disk (Python needs file paths)
                for (int i = 0; i < pdfs.Count; i++)
                {
                    string filePath = Path.Combine(tempFolder, $"{i}_{fileNames[i]}");
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await pdfs[i].CopyToAsync(fileStream, cancellationToken);
                    }
                    savedPaths.Add(filePath);
                }

                await Task.Run(() => RunPyScript(savedPaths, outputPath), cancellationToken);

                var resultStream = new MemoryStream();
                using (var mergedFile = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                {
                    await mergedFile.CopyToAsync(resultStream, cancellationToken);
                }
                resultStream.Position = 0;

                return new ConversionStreamResult(
                    OutputStream: resultStream,
                    FileName: outputFileName,
                    ContentType: "application/pdf",
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
        private static string BuildOutputFileName(List<string> imageFileNames)
        {
            var namesWithoutExtension = imageFileNames
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            string combined = string.Join("_", namesWithoutExtension);

            // Only use combined names if they look meaningful (not generic camera/screenshot names)
            bool looksGeneric = namesWithoutExtension.All(name =>
                name.StartsWith("IMG_") ||
                name.StartsWith("Screenshot") ||
                name.StartsWith("photo") ||
                System.Text.RegularExpressions.Regex.IsMatch(name, @"^\d+$")); // pure numbers like "20240512"

            if (!looksGeneric && imageFileNames.Count <= 3 && combined.Length <= 50)
            {
                return combined + ".pdf";
            }

            return $"images_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        }

        private static void RunPyScript(List<string> pdfFilePaths, string outputPath)
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

                using var pyPathsList = new PyList();
                foreach (var path in pdfFilePaths)
                {
                    pyPathsList.Append(new PyString(path));
                }

                var output = new PyString(outputPath);

                var pythonScript = Py.Import("ImagesToPdf");
                pythonScript.InvokeMethod("ConvertImagesToPdf", new PyObject[] { pyPathsList, output });
            }

        }
    }
}

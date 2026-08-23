using FileConverter.Application.Interfaces.DocumentServiceInterfaces;
using FileConverter.Domain.Models;
using Python.Runtime;

namespace FileConverter.Infrastracture.Services.Document
{
    public class MergePdfsService : IMergePdfsService
    {
        public async Task<ConversionStreamResult> MergePdfsAsync(
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
                // 1. ذخیره فایل‌ها در دیسک برای دسترسی پایتون
                for (int i = 0; i < pdfs.Count; i++)
                {
                    string filePath = Path.Combine(tempFolder, $"{i}_{fileNames[i]}");
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await pdfs[i].CopyToAsync(fileStream, cancellationToken);
                    }
                    savedPaths.Add(filePath);
                }

                // 2. اجرای اسکریپت پایتون
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

        private static string BuildOutputFileName(List<string> fileNames)
        {
            var namesWithoutExtension = fileNames
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            string combined = string.Join("_", namesWithoutExtension);

            // اگر نام خیلی طولانی شد یا تعداد فایل‌ها زیاد بود، نام عمومی می‌دهیم
            if (fileNames.Count <= 3 && combined.Length <= 50)
            {
                return combined + ".pdf";
            }

            return $"Merged_Pdfs_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        }

        private static void RunPyScript(List<string> pdfFilePaths, string outputPath)
        {
            // از مقداردهی اولیه سراسری (در Program.cs) استفاده می‌کنیم
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"D:\Project\C#\File-Converter-Backend\File-Conventer\FileConverter.Infrastracture\Document\PythonScripts");

                using var pyPathsList = new PyList();
                foreach (var path in pdfFilePaths)
                {
                    pyPathsList.Append(new PyString(path));
                }

                var output = new PyString(outputPath);

                var pythonScript = Py.Import("MergePdf");
                pythonScript.InvokeMethod("MergePdfs", new PyObject[] { pyPathsList, output });
            }
        }
    }
}
using FileConverter.Application.Interfaces.ImageServiceInterfaces;
using FileConverter.Domain.Enums;
using FileConverter.Domain.Models;
using Python.Runtime;

namespace FileConverter.Infrastracture.Services.Images;

public class ConvertImageSevice : IConvertImageSevice
{
    public async Task<ConversionStreamResult> ConvertStreamImageAsync(
        Stream inputImage,
        string originalFileName,
        string targetExtension,
        CancellationToken cancellationToken)
    {
        string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempFolder);

        string imageFilePath = Path.Combine(tempFolder, originalFileName);
        string outputFileName = Path.GetFileNameWithoutExtension(originalFileName) + "." + targetExtension;
        string outputPath = Path.Combine(tempFolder, outputFileName);

        try
        {
            // 2. Save the incoming stream to a real file on disk (Python needs a file path)
            using (var fileStream = new FileStream(imageFilePath, FileMode.Create, FileAccess.Write))
            {
                await inputImage.CopyToAsync(fileStream, cancellationToken);
            }

            await Task.Run(() => RunPyScript(imageFilePath, outputPath, targetExtension), cancellationToken);

            // 4. Read the converted image back into memory so we can return it
            var resultStream = new MemoryStream();
            using (var convertedFile = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
            {
                await convertedFile.CopyToAsync(resultStream, cancellationToken);
            }
            resultStream.Position = 0;

            // ✅ تعیین ContentType صحیح و پویا بر اساس فرمت خروجی تصویر
            string contentType = targetExtension.ToLower() switch
            {
                "webp" => "image/webp",
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "bmp" => "image/bmp",
                "gif" => "image/gif",
                "ico" => "image/x-icon",
                _ => "application/octet-stream"
            };

            return new ConversionStreamResult(
                OutputStream: resultStream,
                FileName: outputFileName,
                ContentType: contentType, // ✅ اصلاح شد
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

    private static void RunPyScript(string imageFilePath, string outputPath, string targetExtension)
    {
        if (!PythonEngine.IsInitialized)
        {
            Runtime.PythonDLL = @"C:\Users\Webhouse\AppData\Local\Programs\Python\Python314\python314.dll";
            PythonEngine.Initialize();
        }

        using (Py.GIL())
        {
            dynamic sys = Py.Import("sys");
            
            // ✅ اصلاح مسیر پایتون مشابه سرویس‌های دیگر (بر اساس ساختار پروژه شما)
            sys.path.append(@"D:\Project\C#\File-Converter-Backend\File-Conventer\FileConverter.Infrastracture\Images\PythonScripts");

            var image = new PyString(imageFilePath);
            var output = new PyString(outputPath);
            var format = new PyString(targetExtension);

            var pythonScript = Py.Import("ImageConverter");
            pythonScript.InvokeMethod("ConvertImage", new PyObject[] { image, output, format });
        }
    }
}
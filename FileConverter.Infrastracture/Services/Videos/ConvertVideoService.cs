using FileConverter.Application.Interfaces.VideoServiceInterfaces;
using FileConverter.Domain.Models;
using Python.Runtime;

namespace FileConverter.Infrastracture.Services.Videos
{
    public class ConvertVideoService : IConvertVideoService
    {
        public async Task<ConversionStreamResult> ConvertStreamImageAsync(
            Stream inputvideo,
            string originalFileName,
            string targetExtension,
            CancellationToken cancellationToken)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            string videoFilePath = Path.Combine(tempFolder, originalFileName);
            string outputFileName = Path.GetFileNameWithoutExtension(originalFileName) + "." + targetExtension;
            string outputPath = Path.Combine(tempFolder, outputFileName);

            try
            {
                // Save the incoming stream to a real file on disk
                using (var fileStream = new FileStream(videoFilePath, FileMode.Create, FileAccess.Write))
                {
                    await inputvideo.CopyToAsync(fileStream, cancellationToken);
                }

                // ارسال cancellationToken به Task.Run
                await Task.Run(() => RunPyScript(videoFilePath, outputPath, targetExtension), cancellationToken);

                // Read the converted video back into memory
                var resultStream = new MemoryStream();
                using (var convertedFile = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                {
                    await convertedFile.CopyToAsync(resultStream, cancellationToken);
                }
                resultStream.Position = 0;

                // 💡 تولید ContentType پویا بر اساس فرمت درخواستی
                string contentType = targetExtension.ToLower() switch
                {
                    "mp4" => "video/mp4",
                    "mkv" => "video/x-matroska",
                    "webm" => "video/webm",
                    "avi" => "video/x-msvideo",
                    "mov" => "video/quicktime",
                    "gif" => "image/gif",
                    _ => "application/octet-stream"
                };

                return new ConversionStreamResult(
                    OutputStream: resultStream,
                    FileName: outputFileName,
                    ContentType: contentType, // 💡 به جای فرمت ثابت عکس، از این متغیر استفاده می‌شود
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

        private static void RunPyScript(string videoFilePath, string outputPath, string targetExtension)
        {
            if (!PythonEngine.IsInitialized)
            {
                Runtime.PythonDLL = @"C:\Users\Webhouse\AppData\Local\Programs\Python\Python314\python314.dll";
                PythonEngine.Initialize();
            }

            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(@"D:\Project\C#\File-Converter-Backend\File-Conventer\FileConverter.Infrastracture\Services\Videos\PythonScripts");
                    var video = new PyString(videoFilePath);
                    var output = new PyString(outputPath);
                    var format = new PyString(targetExtension);

                    var pythonScript = Py.Import("VideoConverter");
                    pythonScript.InvokeMethod("ConvertVideo", new PyObject[] { video, output, format });
                }
                catch (PythonException pyEx)
                {
                    // 💡 برای ردیابی راحت‌تر ارورهای احتمالی پایتون
                    throw new Exception($"Python script failed: {pyEx.Message}", pyEx);
                }
            }
        }
    }
}
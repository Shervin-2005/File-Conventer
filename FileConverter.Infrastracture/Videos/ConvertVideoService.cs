using FileConverter.Application.Interfaces.VideoServiceInterfaces;
using FileConverter.Domain.Models;
using Python.Runtime;

namespace FileConverter.Infrastracture.Videos
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
                // 2. Save the incoming stream to a real file on disk (Python needs a file path)
                using (var fileStream = new FileStream(videoFilePath, FileMode.Create, FileAccess.Write))
                {
                    await inputvideo.CopyToAsync(fileStream, cancellationToken);
                }

                await Task.Run(() => RunPyScript(videoFilePath, outputPath, targetExtension));

                // 4. Read the converted video back into memory so we can return it
                var resultStream = new MemoryStream();
                using (var convertedFile = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                {
                    await convertedFile.CopyToAsync(resultStream, cancellationToken);
                }
                resultStream.Position = 0;

                return new ConversionStreamResult(
                    OutputStream: resultStream,
                    FileName: outputFileName,
                    ContentType: "application/vnd.openxmlformats-officedocument.wordprocessingml.image",
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
                Runtime.PythonDLL = @"C:\Users\CLINIC\AppData\Local\Programs\Python\Python314\python314.dll";
                PythonEngine.Initialize();
            }

            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(@"E:\Project\File Converter\FileConverter.Infrastracture\Videos\PythonScripts");

                var video = new PyString(videoFilePath);
                var output = new PyString(outputPath);
                var format = new PyString(targetExtension);

                var pythonScript = Py.Import("VideoConverter");
                pythonScript.InvokeMethod("ConvertVideo", new PyObject[] { video, output, format });
            }
        }
    }
}

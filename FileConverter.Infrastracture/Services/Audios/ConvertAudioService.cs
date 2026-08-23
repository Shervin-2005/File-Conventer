using FileConverter.Application.Interfaces.AudioServiceInterfaces;
using FileConverter.Domain.Models;
using Python.Runtime;

namespace FileConverter.Infrastracture.Services.Audios
{
    public class ConvertAudioService : IConvertAudioService
    {
        public async Task<ConversionStreamResult> ConvertStreamAudioAsync(Stream inputAudio,
            string originalFileName,
            string targetExtension,
            CancellationToken cancellationToken)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            string audioFilePath = Path.Combine(tempFolder, originalFileName);
            string outputFileName = Path.GetFileNameWithoutExtension(originalFileName) + "." + targetExtension;
            string outputPath = Path.Combine(tempFolder, outputFileName);

            try
            {
                // Save the incoming stream to a real file on disk (Python needs a file path)
                using (var fileStream = new FileStream(audioFilePath, FileMode.Create, FileAccess.Write))
                {
                    await inputAudio.CopyToAsync(fileStream, cancellationToken);
                }

                await Task.Run(() => RunPyScript(audioFilePath, outputPath, targetExtension), cancellationToken);

                // Read the converted audio back into memory so we can return it
                var resultStream = new MemoryStream();
                using (var convertedFile = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
                {
                    await convertedFile.CopyToAsync(resultStream, cancellationToken);
                }
                resultStream.Position = 0;

                // ✅ تعیین ContentType صحیح بر اساس فرمت خروجی صدا
                string contentType = targetExtension.ToLower() switch
                {
                    "mp3" => "audio/mpeg",
                    "wav" => "audio/wav",
                    "ogg" => "audio/ogg",
                    "m4a" => "audio/mp4",
                    "flac" => "audio/flac",  
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

        private static void RunPyScript(string audioFilePath, string outputPath, string targetExtension)
        {
            // Python engine is already initialized once at app startup (Program.cs).
            // Just acquire the GIL here and run the conversion.
            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    
                    // ✅ مسیر یکپارچه شد مشابه DocxToTextConversionService
                    sys.path.append(@"D:\Project\C#\File-Converter-Backend\File-Conventer\FileConverter.Infrastracture\Audios\PythonScripts");

                    var audio = new PyString(audioFilePath);
                    var output = new PyString(outputPath);
                    var format = new PyString(targetExtension);

                    var pythonScript = Py.Import("AudioConverter");
                    pythonScript.InvokeMethod("ConvertAudio", new PyObject[] { audio, output, format });
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Python script failed: {pyEx.Message}", pyEx);
                }
            }
        }
    }
}
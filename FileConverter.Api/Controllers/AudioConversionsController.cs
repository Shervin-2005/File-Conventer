using FileConverter.Api.Models.AudioConversions;
using FileConverter.Application.UseCases.Interfaces.AudioInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileConverter.Api.Controllers
{
    public class AudioConversionsController : BaseController
    {
        private readonly IConvertAudioUseCase _convertAudioUseCase;

        public AudioConversionsController(IConvertAudioUseCase convertAudioUseCase)
        {
            _convertAudioUseCase = convertAudioUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> ConvertAudio([FromForm] ConvertAudioRequest request, CancellationToken token)
        {
            if (request.File.Length == 0)
            {
                return BadRequest("صدا را انتخاب کنید.");
            }

            await using var stream = request.File.OpenReadStream();

            var result =
                await _convertAudioUseCase.ExecuteAsync(stream, request.File.FileName, request.TargetFormat, token);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return File(
                fileStream: result.OutputStream!,
                contentType: result.ContentType,
                fileDownloadName: result.FileName);
        }
    }
}

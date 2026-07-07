using FileConverter.Api.Models.VideoConversions;
using FileConverter.Application.UseCases.Interfaces.VideoInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileConverter.Api.Controllers
{
    public class VideoConversionsController : BaseController
    {
        private readonly IConvertVideoUsecase _convertVideoUsecase;

        public VideoConversionsController(IConvertVideoUsecase convertVideoUsecase)
        {
            _convertVideoUsecase = convertVideoUsecase;
        }

        [HttpPost]
        public async Task<IActionResult> ConvertVideo([FromForm] ConvertVideoRequest request, CancellationToken token)
        {
            if (request.File.Length == 0)
            {
                return BadRequest("ویدیو را انتخاب کنید.");
            }

            await using var stream = request.File.OpenReadStream();

            var result =
                await _convertVideoUsecase.ExecuteAsync(stream, request.File.FileName, request.TargetFormat, token);
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

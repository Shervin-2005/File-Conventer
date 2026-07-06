using FileConverter.Api.Models.ImageConversions;
using FileConverter.Application.UseCases.Interfaces.ImagesConversionsInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileConverter.Api.Controllers;

public class ImageConversionsController : BaseController
{
    private readonly IConvertImageUseCase _convertImageUseCase;

    public ImageConversionsController(IConvertImageUseCase convertImageUseCase)
    {
        _convertImageUseCase = convertImageUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> ConvertImage([FromForm] ConvertImageRequest request, CancellationToken token)
    {
        if (request.File.Length == 0)
        {
            return BadRequest("عکس را انتخاب کنید.");
        }

        await using var stream = request.File.OpenReadStream();

        var result =
            await _convertImageUseCase.ExecuteAsync(stream, request.File.FileName, request.TargetFormat, token);
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
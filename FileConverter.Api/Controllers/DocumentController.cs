using FileConverter.Api.Models.DocumentConversions;
using FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileConverter.Api.Controllers
{
    public class DocumentController : BaseController
    {
        private readonly IConvertDocumentUseCase _convertDocumentUseCase;
        private readonly IMergeDocumentUsecase _mergeDocumentUseCase;
        private readonly IEncryptDocumentUsecase _encryptDocumentUsecase;
        private readonly IDecryptDocumentUsecase _decryptDocumentUsecase;
        private readonly IConvertImageToPdfUsecase _convertImageToPdfUsecase;

        public DocumentController(IConvertDocumentUseCase convertDocumentUseCase,
                                  IMergeDocumentUsecase mergeDocumentUsecase,
                                  IEncryptDocumentUsecase encryptDocumentUsecase,
                                  IDecryptDocumentUsecase decryptDocumentUsecase,
                                  IConvertImageToPdfUsecase convertImageToPdfUsecase)
        {
            _convertDocumentUseCase = convertDocumentUseCase;
            _mergeDocumentUseCase = mergeDocumentUsecase;
            _encryptDocumentUsecase = encryptDocumentUsecase;
            _decryptDocumentUsecase = decryptDocumentUsecase;
            _convertImageToPdfUsecase = convertImageToPdfUsecase;
        }

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertDocument([FromForm] ConvertDocumentRequest request, CancellationToken token)
        {
            if (request.File.Length == 0)
            {
                return BadRequest("فایل را انتخاب کنید.");
            }

            await using var stream = request.File.OpenReadStream();

            var result =
                await _convertDocumentUseCase.ExecuteAsync(stream, request.File.FileName,
                request.TargetFormat, request.startPage,
                request.endPage, token);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return File(
                fileStream: result.OutputStream!,
                contentType: result.ContentType,
                fileDownloadName: result.FileName);
        }
        [HttpPost("merge")]
        public async Task<IActionResult> MergePdfs([FromForm] MergePdfRequest request, CancellationToken token)
        {
            if(request.Files == null || request.Files.Count < 2 || request.Files.Count > 10)
            {
                return BadRequest("حداقل دو فایل و حداکثر ده PDF برای ادغام انتخاب کنید.");
            }

            var streams = new List<Stream>();
            var fileNames = new List<string>();

            foreach(var file in request.Files) 
            {
                if(file.Length == 0)
                {
                    return BadRequest("این فایل خالی است " + file.FileName);
                }

                streams.Add(file.OpenReadStream());
                fileNames.Add(file.FileName);
            }

            var result = await _mergeDocumentUseCase.ExecuteAsync(streams, fileNames, token);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return File(
            fileStream: result.OutputStream!,
            contentType: result.ContentType,
            fileDownloadName: result.FileName);
        }
        [HttpPost("encrypt")]
        public async Task<IActionResult> EncryptPdf([FromForm] EncryptDecryptPdfRequest request, CancellationToken token)
        {
            if (request.File.Length == 0)
            {
                return BadRequest("فایل را انتخاب کنید.");
            }

            await using var stream = request.File.OpenReadStream();

            var result =
                await _encryptDocumentUsecase.ExecuteAsync(stream, request.File.FileName, request.password, token);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return File(
                fileStream: result.OutputStream!,
                contentType: result.ContentType,
                fileDownloadName: result.FileName);
        }
        [HttpPost("decrypt")]
        public async Task<IActionResult> DecryptPdf([FromForm] EncryptDecryptPdfRequest request, CancellationToken token)
        {
            if (request.File.Length == 0)
            {
                return BadRequest("فایل را انتخاب کنید.");
            }

            await using var stream = request.File.OpenReadStream();

            var result =
                await _decryptDocumentUsecase.ExecuteAsync(stream, request.File.FileName, request.password, token);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return File(
                fileStream: result.OutputStream!,
                contentType: result.ContentType,
                fileDownloadName: result.FileName);
        }
        [HttpPost("convert-images")]
        public async Task<IActionResult> ConvertImagesToPdf([FromForm] ConvertImagesRequest request, CancellationToken token)
        {
            if (request.Files == null || request.Files.Count < 1)
            {
                return BadRequest("حداقل یک عکس انتخاب کنید.");
            }

            var streams = new List<Stream>();
            var fileNames = new List<string>();

            foreach (var file in request.Files)
            {
                if (file.Length == 0)
                {
                    return BadRequest("یکی فایل ها خالی است");
                }
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".png" && extension != ".jpeg" && extension != ".jpg")
                {
                    return BadRequest("همه‌ی فایل‌ها باید عکس باشند.");
                }

                streams.Add(file.OpenReadStream());
                fileNames.Add(file.FileName);
            }

            var result = await _convertImageToPdfUsecase.ExecuteAsync(streams, fileNames, token);

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

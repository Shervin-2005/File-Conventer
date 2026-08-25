using FileConverter.Api.Models.DocumentConversions;
using FileConverter.Application.UseCases.Implementations;
using FileConverter.Application.UseCases.Implementations.DocumentConversionImplementations;
using FileConverter.Application.UseCases.Interfaces;
using FileConverter.Application.UseCases.Interfaces.DocumentConversionInterfaces;
using FileConverter.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;

namespace FileConverter.Api.Controllers
{
    public class DocumentController : BaseController
    {
        private readonly IConvertDocumentUseCase _convertDocumentUseCase;
        private readonly IMergeDocumentUsecase _mergeDocumentUseCase;
        private readonly IEncryptDocumentUsecase _encryptDocumentUsecase;
        private readonly IDecryptDocumentUsecase _decryptDocumentUsecase;
        private readonly IConvertImageToPdfUsecase _convertImageToPdfUsecase;
        private readonly IConversionJobOrchestrator _conversionJobOrchestrator;

        public DocumentController(IConvertDocumentUseCase convertDocumentUseCase,
                                  IMergeDocumentUsecase mergeDocumentUsecase,
                                  IEncryptDocumentUsecase encryptDocumentUsecase,
                                  IDecryptDocumentUsecase decryptDocumentUsecase,
                                  IConvertImageToPdfUsecase convertImageToPdfUsecase,
                                  IConversionJobOrchestrator conversionJobOrchestrator)
        {
            _convertDocumentUseCase = convertDocumentUseCase;
            _mergeDocumentUseCase = mergeDocumentUsecase;
            _encryptDocumentUsecase = encryptDocumentUsecase;
            _decryptDocumentUsecase = decryptDocumentUsecase;
            _convertImageToPdfUsecase = convertImageToPdfUsecase;
            _conversionJobOrchestrator = conversionJobOrchestrator;
        }

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertDocument([FromForm] ConvertDocumentRequest request, CancellationToken token)
        {
            if (request.File.Length == 0)
            {
                return BadRequest("فایل را انتخاب کنید.");
            }

            int userId = User.Identity?.IsAuthenticated == true
                ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                : 0;

            await using var fileBuffer = new FileBufferingReadStream(
                request.File.OpenReadStream(),
                memoryThreshold: 80 * 1024,
                bufferLimit: null,
                tempFileDirectoryAccessor: () => Path.GetTempPath());

            await fileBuffer.DrainAsync(token);

            ConversionJob? job;
            try
            {
                fileBuffer.Position = 0;
                job = await _conversionJobOrchestrator.StartJobAsync(
                    userId, fileBuffer, request.File.FileName,
                    request.File.ContentType, request.TargetFormat.ToString(), token);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }

            fileBuffer.Position = 0;

            var result = await _convertDocumentUseCase.ExecuteAsync(
                fileBuffer, request.File.FileName, request.TargetFormat,
                request.startPage, request.endPage, token);

            if (!result.Success)
            {
                await _conversionJobOrchestrator.FailJobAsync(job, result.ErrorMessage!, token);
                return BadRequest(result.ErrorMessage);
            }

            var outputBuffer = new FileBufferingReadStream(
                result.OutputStream!,
                memoryThreshold: 80 * 1024,
                bufferLimit: null,
                tempFileDirectoryAccessor: () => Path.GetTempPath());

            await outputBuffer.DrainAsync(token);
            outputBuffer.Position = 0;

            if (job is not null)
            {
                await _conversionJobOrchestrator.CompleteJobAsync(
                    job, userId, outputBuffer, result.FileName!, result.ContentType, token);

                outputBuffer.Position = 0;
            }

            return File(outputBuffer, result.ContentType, result.FileName);
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

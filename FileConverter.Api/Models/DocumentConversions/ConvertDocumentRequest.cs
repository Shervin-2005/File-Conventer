using FileConverter.Domain.Enums;
using System.ComponentModel.DataAnnotations;
namespace FileConverter.Api.Models.DocumentConversions;

public class ConvertDocumentRequest
{
    [Required(ErrorMessage = "لطفاً یک فایل انتخاب کنید.")]
    public IFormFile File { get; set; } = null!;
    public int startPage { get; set; } = 0 ;
    public int endPage { get; set; } = int.MaxValue;
    public DocumentFormat TargetFormat { get; set; }
}
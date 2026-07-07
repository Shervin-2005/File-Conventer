using FileConverter.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileConverter.Api.Models.ImageConversions;

public class ConvertImageRequest
{
    [Required(ErrorMessage = "عکس مورد نظر را انتخاب کنید")]
    public IFormFile File { get; set; } = null!;
    public ImageFormat TargetFormat { get; set; }
}
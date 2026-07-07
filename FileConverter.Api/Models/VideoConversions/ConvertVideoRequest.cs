using FileConverter.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileConverter.Api.Models.VideoConversions
{
    public class ConvertVideoRequest
    {
        [Required(ErrorMessage = "ویدیو مورد نظر را انتخاب کنید")]
        public IFormFile File { get; set; } = null!;
        public VideoFormat TargetFormat { get; set; }
    }
}

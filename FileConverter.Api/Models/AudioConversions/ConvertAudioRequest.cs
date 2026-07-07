using FileConverter.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileConverter.Api.Models.AudioConversions
{
    public class ConvertAudioRequest
    {
        [Required(ErrorMessage = "فایل صوتی مورد نظر را انتخاب کنید")]
        public IFormFile File { get; set; } = null!;
        public ImageFormat TargetFormat { get; set; }
    }
}

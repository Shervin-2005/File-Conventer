using System.ComponentModel.DataAnnotations;

namespace FileConverter.Api.Models.DocumentConversions
{
    public class EncryptDecryptPdfRequest
    {
        [Required(ErrorMessage = "لطفاً یک فایل انتخاب کنید.")]
        public IFormFile File { get; set; } = null!;
        [Required(ErrorMessage = "لطفا رمز عبور را وارد کنید")]
        public string password { get; set; } = null!;
    }
}

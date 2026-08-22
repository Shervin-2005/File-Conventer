using System.ComponentModel.DataAnnotations;

namespace FileConverter.Application.DTOs.Auth
{
    public class RequestPhoneChangeDto
    {
        [Required, Phone]
        public string NewPhoneNumber { get; set; } = null!;
    }
}

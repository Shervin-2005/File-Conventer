using System.ComponentModel.DataAnnotations;

namespace FileConverter.Application.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}

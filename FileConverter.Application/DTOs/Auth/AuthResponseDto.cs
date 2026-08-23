using FileConveter.Domain.Enums;

namespace FileConverter.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int UserId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string? UserName { get; set; }
        public string ProfileUrl { get; set; } = null!;
        public UserRole Role { get; set; }
    }
}

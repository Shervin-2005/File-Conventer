using System.ComponentModel.DataAnnotations;

namespace FileConverter.Application.DTOs.Auth
{
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? UserName { get; set; }

        public Stream? Image { get; set; } 

        public string? ImageName { get; set; }

        public string? ContentType { get; set; }
    }
}

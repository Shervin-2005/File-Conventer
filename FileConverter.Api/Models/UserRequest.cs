using System.ComponentModel.DataAnnotations;

namespace FileConverter.Api.Models
{
    public class UserRequest
    {
        [StringLength(100)]
        public string? UserName { get; set; }

        public IFormFile? File { get; set; }
    }
}

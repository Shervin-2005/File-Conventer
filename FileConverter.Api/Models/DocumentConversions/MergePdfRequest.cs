using System.ComponentModel.DataAnnotations;

namespace FileConverter.Api.Models.DocumentConversions
{
        public class MergePdfRequest
        {
           [Required]
            public List<IFormFile> Files { get; set; } = new();
        }
}

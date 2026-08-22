using FileConveter.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileConverter.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string? PasswordHash {  get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string ProfileUrl { get; set; } = null!;
        public Guid ImageFolderId { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
        public bool IsDeleted {  get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ConversionJob> ConversionJobs { get; set; } = new List<ConversionJob>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}

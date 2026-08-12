using System.ComponentModel.DataAnnotations;

namespace FileConverter.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string? PasswordHash {  get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Role {  get; set; } = "Customer";
        public bool IsDeleted {  get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}

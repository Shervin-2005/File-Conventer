using FileConverter.Domain.Enums;

namespace FileConverter.Domain.Entities
{
    public class ConversionJob
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string? OriginalFileUrl { get; set; }
        public string? OutputFileUrl { get; set; }
        public MediaCategory SourceCategory { get; set; }
        public string SourceFormat { get; set; } = null!;
        public MediaCategory TargetCategory { get; set; }
        public string TargetFormat { get; set; } = null!;
        public ConversionStatus Status { get; set; } = ConversionStatus.Pending;
        public Guid FolderId { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public enum ConversionStatus
        {
            Pending,
            Processing,
            Completed,
            Failed,
            Cancelled,
            TimedOut
        }
    }
}

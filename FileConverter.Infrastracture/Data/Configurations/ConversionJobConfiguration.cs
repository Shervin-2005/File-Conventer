using FileConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileConverter.Infrastracture.Data.Configurations
{
    public class ConversionJobConfiguration : IEntityTypeConfiguration<ConversionJob>
    {
        public void Configure(EntityTypeBuilder<ConversionJob> builder)
        {
            builder.HasKey(j => j.Id);

            // Store enums as their string names
            builder.Property(j => j.SourceCategory)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(j => j.TargetCategory)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(j => j.TargetFormat)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(j => j.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(j => j.ErrorMessage).HasMaxLength(2000);

            builder.HasIndex(j => j.UserId);
            builder.HasIndex(j => j.Status); 

            builder.HasOne(j => j.User)
                .WithMany(u => u.ConversionJobs)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}

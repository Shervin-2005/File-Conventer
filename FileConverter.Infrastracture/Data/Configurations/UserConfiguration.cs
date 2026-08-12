using FileConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileConverter.Infrastracture.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(u => u.UserName).HasMaxLength(100);

            builder.HasIndex(u => u.PhoneNumber).IsUnique();
        }
    }
}

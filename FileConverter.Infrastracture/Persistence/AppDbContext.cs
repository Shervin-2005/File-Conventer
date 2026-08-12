using FileConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileConverter.Infrastracture.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<ConversionJob> ConversionJobs => Set<ConversionJob>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.PhoneNumber).IsUnique();

                entity.HasQueryFilter(u => !u.IsDeleted);
            });

            modelBuilder.Entity<ConversionJob>(entity =>
            {
                // Store enums as their string names
                entity.Property(j => j.SourceCategory).HasConversion<string>().HasMaxLength(20);
                entity.Property(j => j.TargetCategory).HasConversion<string>().HasMaxLength(20);
                entity.Property(j => j.Status).HasConversion<string>().HasMaxLength(20);

                entity.HasIndex(j => j.UserId);
                entity.HasIndex(j => j.Status);

                entity.HasOne(j => j.User)
                      .WithMany(u => u.ConversionJobs)
                      .HasForeignKey(j => j.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}

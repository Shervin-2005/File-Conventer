using FileConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileConverter.Infrastracture.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<OtpVerification> OtpVerifications { get; set; } = null!;
        public DbSet<ConversionJob> ConversionJobs => Set<ConversionJob>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

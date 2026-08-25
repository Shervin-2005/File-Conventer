using FileConverter.Infrastracture.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FileConverter.Infrastracture.Services
{
    public class ConversionJobCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ConversionJobCleanupService> _logger;
        private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(1);
        private static readonly TimeSpan Retention = TimeSpan.FromHours(24);

        public ConversionJobCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<ConversionJobCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredJobsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Conversion job cleanup failed.");
                }

                await Task.Delay(CleanupInterval, stoppingToken);
            }
        }

        private async Task CleanupExpiredJobsAsync(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cutoff = DateTime.UtcNow - Retention;

            var expiredJobs = await db.ConversionJobs
                .Where(j => j.CreatedAt < cutoff)
                .ToListAsync(token);

            if (expiredJobs.Count == 0) return;

            db.ConversionJobs.RemoveRange(expiredJobs);
            await db.SaveChangesAsync(token);

            _logger.LogInformation("Cleaned up {Count} expired conversion jobs.", expiredJobs.Count);
        }
    }
}

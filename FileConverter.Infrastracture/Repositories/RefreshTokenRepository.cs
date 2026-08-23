using FileConverter.Application.Interfaces.AuthInterfaces;
using FileConverter.Domain.Entities;
using FileConverter.Infrastracture.Data;
using FileConveter.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash) =>
            await _dbSet.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

        public async Task RevokeAllForUserAsync(int userId)
        {
            var activeTokens = await _dbSet
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }
        }
    }
}

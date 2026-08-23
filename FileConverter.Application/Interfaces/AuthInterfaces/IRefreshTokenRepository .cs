using FileConverter.Domain.Entities;

namespace FileConverter.Application.Interfaces.AuthInterfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task RevokeAllForUserAsync(int userId);
    }
}

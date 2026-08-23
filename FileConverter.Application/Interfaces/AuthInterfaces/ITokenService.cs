using System.Security.Claims;

namespace FileConverter.Application.Interfaces.AuthInterfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
    }
}

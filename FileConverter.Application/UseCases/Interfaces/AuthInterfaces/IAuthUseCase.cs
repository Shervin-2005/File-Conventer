using FileConverter.Application.DTOs.Auth;
using FileConverter.Domain.Enums;

namespace FileConverter.Application.UseCases.Interfaces.AuthInterfaces
{
    public interface IAuthUseCase
    {
        Task RequestRegistrationOtpAsync(RequestOtpDto dto);
        Task<AuthResponseDto> VerifyRegistrationOtpAsync(VerifyRegistrationOtpDto dto);
        Task RequestLoginOtpAsync(RequestOtpDto dto);
        Task<AuthResponseDto?> VerifyLoginWithOtpAsync(LoginWithOtpDto dto);
        Task<AuthResponseDto?> LoginWithPasswordAsync(LoginWithPasswordDto dto);
        Task RequestPhoneChangeAsync(int userId, RequestPhoneChangeDto dto);
        Task<bool> VerifyPhoneChangeAsync(int userId, VerifyPhoneChangeDto dto);
        Task RequestAddPasswordOtpAsync(int userId);
        Task<AddPasswordResult> VerifyAddPasswordAsync(int userId, string otpCode, string newPassword);
        Task<ChangePasswordResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto);
        Task<AuthResponseDto> ReissueTokensAsync(int userId);
        Task LogoutAsync(RefreshTokenDto dto);
    }
}
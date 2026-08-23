using FileConverter.Domain.Enums;

namespace FileConverter.Application.UseCases.Interfaces.AuthInterfaces
{
    public interface IOtpUseCase
    {
        Task IssueOtpAsync(string phoneNumber, OtpPurpose purpose);
        Task<bool> ConsumeOtpAsync(string phoneNumber, string code);
    }
}

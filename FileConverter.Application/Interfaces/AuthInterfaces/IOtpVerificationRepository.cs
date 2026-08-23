using FileConverter.Domain.Entities;

namespace FileConverter.Application.Interfaces.AuthInterfaces
{
    public interface IOtpVerificationRepository : IGenericRepository<OtpVerification>
    {
        Task<OtpVerification?> GetLatestAsync(string phoneNumber);
    }
}

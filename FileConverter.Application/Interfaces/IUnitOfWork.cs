using FileConverter.Application.Interfaces.AuthInterfaces;

namespace FileConverter.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IOtpVerificationRepository OtpVerifications { get; }
        IRefreshTokenRepository? RefreshTokens {  get; }
        IUserRepository Users {  get; }
        IConversionJobHistoryRepository ConversionJobs {  get; }


        Task<int> SaveChangesAsync();
    }
}
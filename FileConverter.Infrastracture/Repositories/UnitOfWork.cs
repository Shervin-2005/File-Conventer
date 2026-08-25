using FileConverter.Application.Interfaces;
using FileConverter.Application.Interfaces.AuthInterfaces;
using FileConverter.Infrastracture.Data;
using FileConverter.Infrastracture.Repositories;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IUserRepository _userRepository;
        private IOtpVerificationRepository? _otpVerifications;
        private IRefreshTokenRepository? _refreshTokens;
        private IConversionJobHistoryRepository? _conversionJobs;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IOtpVerificationRepository OtpVerifications =>
            _otpVerifications ??= new OtpVerificationRepository(_context);
        
        public IRefreshTokenRepository RefreshTokens =>
            _refreshTokens ??= new RefreshTokenRepository(_context);

        public IConversionJobHistoryRepository ConversionJobs =>
            _conversionJobs ??= new ConversionJobHistoryRepository(_context);



        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
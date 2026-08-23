using FileConverter.Domain.Entities;

namespace FileConverter.Application.Interfaces.AuthInterfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<User>> GetAllUsersWithoutFilter();
        Task<User?> GetByUserNameAsync(string UserName);
    }
}

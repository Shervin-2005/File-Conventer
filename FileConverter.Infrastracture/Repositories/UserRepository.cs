using FileConverter.Application.Interfaces.AuthInterfaces;
using FileConverter.Domain.Entities;
using FileConverter.Infrastracture.Data;
using FileConveter.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber) =>
            await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        public async Task<IEnumerable<User>> GetAllUsersWithoutFilter() =>
            await _dbSet.IgnoreQueryFilters().ToListAsync();

        public async Task<User?> GetByUserNameAsync(string UserName)=>
            await _dbSet.FirstOrDefaultAsync(u => u.UserName == UserName);
    }
}

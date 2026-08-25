using Microsoft.EntityFrameworkCore;
using FileConverter.Application.Interfaces;
using FileConverter.Domain.Entities;
using FileConverter.Infrastracture.Data;
using FileConveter.Infrastructure.Repositories;

namespace FileConverter.Infrastracture.Repositories
{
    public class ConversionJobHistoryRepository : GenericRepository<ConversionJob>, IConversionJobHistoryRepository
    {
        public ConversionJobHistoryRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ConversionJob>> GetByUserIdAsync(int userId) =>
           await _dbSet.Where(j => j.UserId == userId)
                         .OrderByDescending(j => j.CreatedAt)
                         .ToListAsync();
    }
}

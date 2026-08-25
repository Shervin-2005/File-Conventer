using FileConverter.Domain.Entities;

namespace FileConverter.Application.Interfaces
{
    public interface IConversionJobHistoryRepository: IGenericRepository<ConversionJob>
    {
        Task<IEnumerable<ConversionJob>> GetByUserIdAsync(int userId);
    }
}

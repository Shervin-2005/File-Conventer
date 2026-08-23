using FileConverter.Application.DTOs.Auth;

namespace FileConverter.Application.UseCases.Interfaces.AuthInterfaces
{
    public interface IUserUseCase
    {
        Task<UserDto?> GetByIdAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UpdateUserDto dto);
        Task<bool> SoftDeleteAsync(int userId);
        Task<IEnumerable<UserDto>> GetAllActiveUsersAsync();
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}

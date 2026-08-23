using AutoMapper;
using FileConverter.Application.DTOs.Auth;
using FileConverter.Application.Interfaces;
using FileConverter.Application.UseCases.Interfaces.AuthInterfaces;

namespace FileConverter.Application.UseCases.Implementations.AuthImplementations
{
    public class UserUseCase : IUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOtpUseCase _otpUseCase;
        private readonly IImageStorageService _imageStorageService;

        public UserUseCase(IUnitOfWork unitOfWork, IMapper mapper, IImageStorageService imageStorageService, IOtpUseCase otpUseCase)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
            _otpUseCase = otpUseCase;
        }

        public async Task<UserDto?> GetByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user is null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<bool> SoftDeleteAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) return false;

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.RefreshTokens.RevokeAllForUserAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllActiveUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllUsersWithoutFilter();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateUserDto dto)
        {
            string profileUrl = "";
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user is null) return false;

                if (dto.UserName is not null && !string.Equals(user.UserName, dto.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    var existingUser = await _unitOfWork.Users.GetByUserNameAsync(dto.UserName);

                    if (existingUser is not null)
                        throw new InvalidOperationException("Username is already taken.");

                    user.UserName = dto.UserName;
                }

                var oldProfileUrl = user.ProfileUrl;
                var imageProvided = dto.Image is not null;

                if (imageProvided)
                {
                    var extension = Path.GetExtension(dto.ImageName);
                    var imageName = $"profile{extension}";

                    profileUrl = await _imageStorageService.UploadAsync(dto.Image!, $"User/{user.ImageFolderId}/images",
                                                                          imageName, dto.ContentType!);
                    user.ProfileUrl = profileUrl;
                }

                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                if (imageProvided && !string.Equals(oldProfileUrl, profileUrl, StringComparison.OrdinalIgnoreCase))
                {
                    await _imageStorageService.DeleteAsync(oldProfileUrl);
                }
                return true;
            }
            catch
            {
                if (!string.IsNullOrEmpty(profileUrl))
                {
                    await _imageStorageService.DeleteAsync(profileUrl);
                }
                throw;
            }
        }
    }
}

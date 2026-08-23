using FileConverter.Api.Models;
using FileConverter.Application.DTOs.Auth;
using FileConverter.Application.UseCases.Interfaces.AuthInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FileConverter.Api.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserUseCase _userUseCase;

        public UserController(IUserUseCase userUseCase)
        {
            _userUseCase = userUseCase;
        }
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var user = await _userUseCase.GetByIdAsync(GetUserId());
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe(UserRequest request)
        {
            Stream? imageStream = null;

            try
            {
                //later should alter this with fluent validation 
                if (request.File is not null && request.File.Length > 0)
                {
                    const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
                    if (request.File.Length > maxSizeBytes)
                        return BadRequest("File too large. Max size is 5 MB.");

                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
                    if (!allowedTypes.Contains(request.File.ContentType))
                        return BadRequest("Unsupported file type. Use JPEG, PNG, or WebP.");

                    imageStream = request.File.OpenReadStream();
                }

                var dto = new UpdateUserDto
                {
                    UserName = request.UserName,
                    Image = imageStream,
                    ImageName = request.File?.FileName,
                    ContentType = request.File?.ContentType
                };

                var updated = await _userUseCase.UpdateProfileAsync(GetUserId(), dto);
                if (!updated) return NotFound();
                return NoContent();
            }
            finally
            {
                if (imageStream is not null)
                    await imageStream.DisposeAsync();
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userUseCase.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllActiveUsers()
        {
            var users = await _userUseCase.GetAllActiveUsersAsync();
            return Ok(users);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userUseCase.GetAllUsersAsync();
            return Ok(users);
        }

        private int GetUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(value, out var userId))
                throw new UnauthorizedAccessException("Invalid user identity.");

            return userId;
        }
    }
}


using FileConverter.Application.Interfaces.AuthInterfaces;
using FileConverter.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        private readonly PasswordHasher<User> _hasher = new();

        public string Hash(string password) =>
            _hasher.HashPassword(null!, password);

        public bool Verify(string hashedPassword, string providedPassword) =>
            _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
    }
}
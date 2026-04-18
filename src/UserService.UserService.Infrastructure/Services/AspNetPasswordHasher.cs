using Microsoft.AspNetCore.Identity;
using UserService.Domain.Interfaces;

namespace UserService.Infrastructure.Services;

// PBKDF2 с солью
public class AspNetPasswordHasher : IPasswordHasher
{
    private readonly IPasswordHasher<object> _hasher = new PasswordHasher<object>();

    public string Hash(string password) => _hasher.HashPassword(null!, password);
    
    public bool Verify(string hashedPassword, string providedPassword) =>
        _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword) 
        == PasswordVerificationResult.Success;
}
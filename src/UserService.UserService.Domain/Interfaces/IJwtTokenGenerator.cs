namespace UserService.Domain.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string name);
}
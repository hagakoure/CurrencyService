using MediatR;
using UserService.Application.Common;
using UserService.Domain.Interfaces;

namespace UserService.Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByNameAsync(request.Name, ct);
        if (user == null) return Result<string>.Failure("Пользователь не найден");

        if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
            return Result<string>.Failure("Неверный пароль");

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Name);
        return Result<string>.Success(token);
    }
}
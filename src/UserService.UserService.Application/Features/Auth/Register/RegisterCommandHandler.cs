using MediatR;
using UserService.Application.Common;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Features.Auth.Register;

public class RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await userRepository.ExistsByNameAsync(request.Name, ct))
            return Result<Guid>.Failure("Пользователь с таким именем уже существует");

        var hash = passwordHasher.Hash(request.Password);

        var user = User.Create(request.Name, hash);

        await userRepository.AddAsync(user, ct);
        await userRepository.SaveChangesAsync(ct);

        return Result<Guid>.Success(user.Id);
    }
}
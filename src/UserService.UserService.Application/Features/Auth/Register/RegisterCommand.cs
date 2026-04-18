using MediatR;
using UserService.Application.Common;

namespace UserService.Application.Features.Auth.Register;

public record RegisterCommand(string Name, string Password) : IRequest<Result<Guid>>;
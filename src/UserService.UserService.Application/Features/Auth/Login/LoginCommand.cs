using MediatR;
using UserService.Application.Common;

namespace UserService.Application.Features.Auth.Login;

public record LoginCommand(string Name, string Password) : IRequest<Result<string>>;
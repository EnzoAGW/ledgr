using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(string Token, int UserId, string Name, UserRole Role, int OrgId);

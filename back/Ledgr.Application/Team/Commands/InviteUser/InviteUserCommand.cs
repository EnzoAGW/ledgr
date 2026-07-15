using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Team.Commands.InviteUser;

public record InviteUserCommand(int OrgId, UserRole InviterRole, string Name, string Email, string Password, UserRole Role) : IRequest<int>;

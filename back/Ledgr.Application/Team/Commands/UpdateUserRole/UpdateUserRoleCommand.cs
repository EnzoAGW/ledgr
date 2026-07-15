using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Team.Commands.UpdateUserRole;

public record UpdateUserRoleCommand(int TargetUserId, int OrgId, int RequesterId, UserRole NewRole) : IRequest;

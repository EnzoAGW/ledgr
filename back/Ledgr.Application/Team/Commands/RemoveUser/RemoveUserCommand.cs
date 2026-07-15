using MediatR;

namespace Ledgr.Application.Team.Commands.RemoveUser;

public record RemoveUserCommand(int TargetUserId, int OrgId, int RequesterId) : IRequest;

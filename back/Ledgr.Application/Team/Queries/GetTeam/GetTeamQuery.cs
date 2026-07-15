using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Team.Queries.GetTeam;

public record GetTeamQuery(int OrgId) : IRequest<IList<TeamMemberDto>>;

public record TeamMemberDto(int Id, string Name, string Email, UserRole Role);

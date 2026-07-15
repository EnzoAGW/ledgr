using Ledgr.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Team.Queries.GetTeam;

public class GetTeamQueryHandler(ILedgrDbContext db) : IRequestHandler<GetTeamQuery, IList<TeamMemberDto>>
{
    public async Task<IList<TeamMemberDto>> Handle(GetTeamQuery req, CancellationToken ct)
    {
        return await db.Users
            .Where(u => u.OrgId == req.OrgId)
            .OrderBy(u => u.Role).ThenBy(u => u.Name)
            .Select(u => new TeamMemberDto(u.Id, u.Name, u.Email, u.Role))
            .ToListAsync(ct);
    }
}

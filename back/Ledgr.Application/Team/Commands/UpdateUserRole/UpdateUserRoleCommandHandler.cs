using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Team.Commands.UpdateUserRole;

public class UpdateUserRoleCommandHandler(ILedgrDbContext db) : IRequestHandler<UpdateUserRoleCommand>
{
    public async Task Handle(UpdateUserRoleCommand req, CancellationToken ct)
    {
        if (req.TargetUserId == req.RequesterId)
            throw new ForbiddenException("Cannot change your own role.");

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == req.TargetUserId && u.OrgId == req.OrgId, ct)
            ?? throw new NotFoundException("User not found.");

        user.Role = req.NewRole;
        await db.SaveChangesAsync(ct);
    }
}

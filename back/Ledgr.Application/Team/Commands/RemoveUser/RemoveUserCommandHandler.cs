using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Team.Commands.RemoveUser;

public class RemoveUserCommandHandler(ILedgrDbContext db) : IRequestHandler<RemoveUserCommand>
{
    public async Task Handle(RemoveUserCommand req, CancellationToken ct)
    {
        if (req.TargetUserId == req.RequesterId)
            throw new ForbiddenException("Cannot remove yourself.");

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == req.TargetUserId && u.OrgId == req.OrgId, ct)
            ?? throw new NotFoundException("User not found.");

        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }
}

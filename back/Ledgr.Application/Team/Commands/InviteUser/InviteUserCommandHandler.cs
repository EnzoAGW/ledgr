using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Entities;
using Ledgr.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Team.Commands.InviteUser;

public class InviteUserCommandHandler(ILedgrDbContext db, IPasswordHasher hasher) : IRequestHandler<InviteUserCommand, int>
{
    public async Task<int> Handle(InviteUserCommand req, CancellationToken ct)
    {
        // Role hierarchy: Admin can invite any role; Manager can only invite Analyst
        if (req.InviterRole == UserRole.Manager && req.Role != UserRole.Analyst)
            throw new ForbiddenException("Managers can only invite Analysts.");

        if (req.InviterRole == UserRole.Analyst)
            throw new ForbiddenException("Analysts cannot invite users.");

        if (await db.Users.AnyAsync(u => u.Email == req.Email, ct))
            throw new ConflictException("Email already in use.");

        var user = new User
        {
            OrgId        = req.OrgId,
            Name         = req.Name,
            Email        = req.Email,
            PasswordHash = hasher.Hash(req.Password),
            Role         = req.Role,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user.Id;
    }
}

using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Entities;
using Ledgr.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Auth.Commands.RegisterOrg;

public class RegisterOrgCommandHandler(ILedgrDbContext db, IPasswordHasher hasher) : IRequestHandler<RegisterOrgCommand, int>
{
    public async Task<int> Handle(RegisterOrgCommand req, CancellationToken ct)
    {
        if (await db.Users.AnyAsync(u => u.Email == req.Email, ct))
            throw new ConflictException("Email already in use.");

        var org = new Organization { Name = req.OrgName };
        db.Organizations.Add(org);

        var admin = new User
        {
            Name         = req.AdminName,
            Email        = req.Email,
            PasswordHash = hasher.Hash(req.Password),
            Role         = UserRole.Admin,
            Org          = org,
        };
        db.Users.Add(admin);

        await db.SaveChangesAsync(ct);
        return org.Id;
    }
}

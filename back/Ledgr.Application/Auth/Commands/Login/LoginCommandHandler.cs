using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Auth.Commands.Login;

public class LoginCommandHandler(ILedgrDbContext db, IJwtService jwt, IPasswordHasher hasher) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand req, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct)
            ?? throw new UnauthorizedException();

        if (!hasher.Verify(req.Password, user.PasswordHash))
            throw new UnauthorizedException();

        return new LoginResult(jwt.Generate(user), user.Id, user.Name, user.Role, user.OrgId);
    }
}

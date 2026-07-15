using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Entities;
using MediatR;

namespace Ledgr.Application.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler(ILedgrDbContext db) : IRequestHandler<CreateAccountCommand, int>
{
    public async Task<int> Handle(CreateAccountCommand req, CancellationToken ct)
    {
        var account = new Account { OrgId = req.OrgId, Name = req.Name, Type = req.Type };
        db.Accounts.Add(account);
        await db.SaveChangesAsync(ct);
        return account.Id;
    }
}

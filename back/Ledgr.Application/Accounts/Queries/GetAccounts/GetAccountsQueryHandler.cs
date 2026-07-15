using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Accounts.Queries.GetAccounts;

public class GetAccountsQueryHandler(ILedgrDbContext db) : IRequestHandler<GetAccountsQuery, IList<AccountDto>>
{
    public async Task<IList<AccountDto>> Handle(GetAccountsQuery req, CancellationToken ct)
    {
        return await db.Accounts
            .Where(a => a.OrgId == req.OrgId)
            .Select(a => new AccountDto(
                a.Id,
                a.Name,
                a.Type,
                a.Transactions
                    .Where(t => t.Status == TransactionStatus.Confirmed)
                    .Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount)
            ))
            .ToListAsync(ct);
    }
}

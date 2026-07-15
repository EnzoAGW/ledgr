using Ledgr.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Transactions.Queries.GetTransactions;

public class GetTransactionsQueryHandler(ILedgrDbContext db) : IRequestHandler<GetTransactionsQuery, PagedResult<TransactionDto>>
{
    public async Task<PagedResult<TransactionDto>> Handle(GetTransactionsQuery req, CancellationToken ct)
    {
        var query = db.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Where(t => t.OrgId == req.OrgId);

        if (!string.IsNullOrWhiteSpace(req.Search))
            query = query.Where(t => t.Description != null && t.Description.Contains(req.Search));

        if (req.AccountId.HasValue)  query = query.Where(t => t.AccountId == req.AccountId);
        if (req.CategoryId.HasValue) query = query.Where(t => t.CategoryId == req.CategoryId);
        if (req.Type.HasValue)       query = query.Where(t => t.Type == req.Type);
        if (req.Status.HasValue)     query = query.Where(t => t.Status == req.Status);
        if (req.From.HasValue)       query = query.Where(t => t.Date >= req.From);
        if (req.To.HasValue)         query = query.Where(t => t.Date <= req.To);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(t => t.Date)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(t => new TransactionDto(
                t.Id, t.Amount, t.Type, t.Status, t.Date, t.Description,
                t.AccountId, t.Account.Name,
                t.CategoryId, t.Category == null ? null : t.Category.Name,
                t.Category == null ? null : t.Category.Color))
            .ToListAsync(ct);

        return new PagedResult<TransactionDto>(items, total, req.Page, req.PageSize);
    }
}

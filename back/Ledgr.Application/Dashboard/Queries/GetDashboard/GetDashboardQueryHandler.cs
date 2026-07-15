using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Dashboard.Queries.GetDashboard;

public class GetDashboardQueryHandler(ILedgrDbContext db) : IRequestHandler<GetDashboardQuery, DashboardResult>
{
    public async Task<DashboardResult> Handle(GetDashboardQuery req, CancellationToken ct)
    {
        var confirmed = await db.Transactions
            .Where(t => t.OrgId == req.OrgId && t.Status == TransactionStatus.Confirmed)
            .ToListAsync(ct);

        var totalIncome  = confirmed.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = confirmed.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        var pendingCount = await db.Transactions
            .CountAsync(t => t.OrgId == req.OrgId && t.Status == TransactionStatus.Pending, ct);

        var cutoff = DateTime.UtcNow.Date.AddDays(-6);
        var recent = confirmed.Where(t => t.Date.Date >= cutoff).ToList();

        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.UtcNow.Date.AddDays(-6 + i))
            .Select(day => new DailyVolume(
                day.ToString("MM/dd"),
                recent.Where(t => t.Date.Date == day && t.Type == TransactionType.Income).Sum(t => t.Amount),
                recent.Where(t => t.Date.Date == day && t.Type == TransactionType.Expense).Sum(t => t.Amount)
            ))
            .ToList();

        var rawCategories = await db.Transactions
            .Where(t => t.OrgId == req.OrgId && t.Status == TransactionStatus.Confirmed
                        && t.Type == TransactionType.Expense && t.CategoryId != null)
            .Select(t => new { t.Amount, Name = t.Category!.Name, Color = t.Category!.Color })
            .ToListAsync(ct);

        var topCategories = rawCategories
            .GroupBy(t => new { t.Name, t.Color })
            .Select(g => new CategoryBreakdown(g.Key.Name, g.Key.Color, g.Sum(t => t.Amount)))
            .OrderByDescending(c => c.Amount)
            .Take(6)
            .ToList();

        return new DashboardResult(totalIncome, totalExpense, totalIncome - totalExpense, pendingCount, last7Days, topCategories);
    }
}

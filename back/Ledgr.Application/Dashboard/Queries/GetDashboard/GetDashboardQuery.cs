using MediatR;

namespace Ledgr.Application.Dashboard.Queries.GetDashboard;

public record GetDashboardQuery(int OrgId) : IRequest<DashboardResult>;

public record DashboardResult(
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance,
    int PendingCount,
    IList<DailyVolume> Last7Days,
    IList<CategoryBreakdown> TopCategories
);

public record DailyVolume(string Date, decimal Income, decimal Expense);
public record CategoryBreakdown(string Name, string Color, decimal Amount);

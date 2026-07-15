using FluentAssertions;
using Ledgr.Application.Dashboard.Queries.GetDashboard;
using Ledgr.Domain.Entities;
using Ledgr.Domain.Enums;
using Ledgr.Infrastructure.Persistence;
using Ledgr.Tests.Helpers;

namespace Ledgr.Tests.Dashboard;

public class GetDashboardQueryHandlerTests
{
    private static async Task<LedgrDbContext> SeedAsync()
    {
        var db = DbContextFactory.Create();

        db.Organizations.Add(new Organization { Id = 1, Name = "Org" });
        db.Organizations.Add(new Organization { Id = 2, Name = "OtherOrg" });
        db.Accounts.Add(new Account { Id = 1, OrgId = 1, Name = "Main", Type = AccountType.Checking });
        db.Accounts.Add(new Account { Id = 2, OrgId = 2, Name = "OtherMain", Type = AccountType.Checking });

        db.Transactions.AddRange(
            // Org 1 — confirmed income
            new Transaction { OrgId = 1, AccountId = 1, Amount = 1000m, Type = TransactionType.Income,  Status = TransactionStatus.Confirmed, Date = DateTime.UtcNow },
            // Org 1 — confirmed expense
            new Transaction { OrgId = 1, AccountId = 1, Amount = 400m,  Type = TransactionType.Expense, Status = TransactionStatus.Confirmed, Date = DateTime.UtcNow },
            // Org 1 — pending expense (must not count towards totals)
            new Transaction { OrgId = 1, AccountId = 1, Amount = 200m,  Type = TransactionType.Expense, Status = TransactionStatus.Pending,   Date = DateTime.UtcNow },
            // Org 2 — must be excluded from Org 1 KPIs
            new Transaction { OrgId = 2, AccountId = 2, Amount = 9999m, Type = TransactionType.Income,  Status = TransactionStatus.Confirmed, Date = DateTime.UtcNow }
        );

        await db.SaveChangesAsync();
        return db;
    }

    [Fact]
    public async Task Handle_ReturnsCorrectIncome()
    {
        using var db = await SeedAsync();
        var result = await new GetDashboardQueryHandler(db).Handle(new GetDashboardQuery(1), CancellationToken.None);
        result.TotalIncome.Should().Be(1000m);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectExpense()
    {
        using var db = await SeedAsync();
        var result = await new GetDashboardQueryHandler(db).Handle(new GetDashboardQuery(1), CancellationToken.None);
        // Only confirmed expense (400), pending (200) excluded
        result.TotalExpense.Should().Be(400m);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectBalance()
    {
        using var db = await SeedAsync();
        var result = await new GetDashboardQueryHandler(db).Handle(new GetDashboardQuery(1), CancellationToken.None);
        result.Balance.Should().Be(600m);
    }

    [Fact]
    public async Task Handle_PendingCountIncludesOnlyPending()
    {
        using var db = await SeedAsync();
        var result = await new GetDashboardQueryHandler(db).Handle(new GetDashboardQuery(1), CancellationToken.None);
        result.PendingCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_OtherOrgDataIsExcluded()
    {
        using var db = await SeedAsync();
        var result = await new GetDashboardQueryHandler(db).Handle(new GetDashboardQuery(1), CancellationToken.None);
        // Org 2 has 9999 income — must not bleed into Org 1
        result.TotalIncome.Should().Be(1000m);
    }

    [Fact]
    public async Task Handle_Last7DaysAlwaysHasSevenEntries()
    {
        using var db = await SeedAsync();
        var result = await new GetDashboardQueryHandler(db).Handle(new GetDashboardQuery(1), CancellationToken.None);
        result.Last7Days.Should().HaveCount(7);
    }

    [Fact]
    public async Task Handle_EmptyOrg_ReturnsZeroKPIs()
    {
        using var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "Empty" });
        await db.SaveChangesAsync();

        var result = await new GetDashboardQueryHandler(db).Handle(new GetDashboardQuery(1), CancellationToken.None);

        result.TotalIncome.Should().Be(0m);
        result.TotalExpense.Should().Be(0m);
        result.Balance.Should().Be(0m);
        result.PendingCount.Should().Be(0);
        result.Last7Days.Should().HaveCount(7);
        result.TopCategories.Should().BeEmpty();
    }
}

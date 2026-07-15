using FluentAssertions;
using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Transactions.Commands.CreateTransaction;
using Ledgr.Domain.Entities;
using Ledgr.Domain.Enums;
using Ledgr.Infrastructure.Persistence;
using Ledgr.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Tests.Transactions;

public class CreateTransactionCommandHandlerTests
{
    private static async Task<(LedgrDbContext db, int orgId, int accountId, int categoryId)> SeedAsync()
    {
        var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "Org" });
        db.Accounts.Add(new Account { Id = 1, OrgId = 1, Name = "Main", Type = AccountType.Checking });
        db.Categories.Add(new Category { Id = 1, OrgId = 1, Name = "Food", Type = CategoryType.Expense, Color = "#111111" });
        await db.SaveChangesAsync();
        return (db, 1, 1, 1);
    }

    [Fact]
    public async Task Handle_ValidTransaction_ReturnsPersistId()
    {
        var (db, orgId, accountId, catId) = await SeedAsync();
        var handler = new CreateTransactionCommandHandler(db);

        var id = await handler.Handle(
            new CreateTransactionCommand(orgId, accountId, catId, 150m, TransactionType.Expense, DateTime.UtcNow, "Lunch"),
            CancellationToken.None);

        id.Should().BeGreaterThan(0);
        (await db.Transactions.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_ValidTransaction_WithoutCategory_Succeeds()
    {
        var (db, orgId, accountId, _) = await SeedAsync();
        var handler = new CreateTransactionCommandHandler(db);

        var id = await handler.Handle(
            new CreateTransactionCommand(orgId, accountId, null, 50m, TransactionType.Income, DateTime.UtcNow, null),
            CancellationToken.None);

        id.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handle_InvalidAmount_ThrowsValidation(decimal amount)
    {
        var (db, orgId, accountId, _) = await SeedAsync();
        var handler = new CreateTransactionCommandHandler(db);

        await handler.Invoking(h => h.Handle(
                new CreateTransactionCommand(orgId, accountId, null, amount, TransactionType.Expense, DateTime.UtcNow, null),
                CancellationToken.None))
            .Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_AccountNotFound_ThrowsNotFound()
    {
        var (db, orgId, _, _) = await SeedAsync();
        var handler = new CreateTransactionCommandHandler(db);

        await handler.Invoking(h => h.Handle(
                new CreateTransactionCommand(orgId, 999, null, 100m, TransactionType.Expense, DateTime.UtcNow, null),
                CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_AccountFromOtherOrg_ThrowsForbidden()
    {
        var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "OrgA" });
        db.Organizations.Add(new Organization { Id = 2, Name = "OrgB" });
        db.Accounts.Add(new Account { Id = 1, OrgId = 2, Name = "OrgB Account", Type = AccountType.Checking });
        await db.SaveChangesAsync();

        var handler = new CreateTransactionCommandHandler(db);

        await handler.Invoking(h => h.Handle(
                new CreateTransactionCommand(1, 1, null, 100m, TransactionType.Expense, DateTime.UtcNow, null),
                CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsNotFound()
    {
        var (db, orgId, accountId, _) = await SeedAsync();
        var handler = new CreateTransactionCommandHandler(db);

        await handler.Invoking(h => h.Handle(
                new CreateTransactionCommand(orgId, accountId, 999, 100m, TransactionType.Expense, DateTime.UtcNow, null),
                CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_CategoryFromOtherOrg_ThrowsForbidden()
    {
        var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "OrgA" });
        db.Organizations.Add(new Organization { Id = 2, Name = "OrgB" });
        db.Accounts.Add(new Account { Id = 1, OrgId = 1, Name = "Account", Type = AccountType.Checking });
        db.Categories.Add(new Category { Id = 1, OrgId = 2, Name = "OtherOrg Cat", Type = CategoryType.Expense, Color = "#222222" });
        await db.SaveChangesAsync();

        var handler = new CreateTransactionCommandHandler(db);

        await handler.Invoking(h => h.Handle(
                new CreateTransactionCommand(1, 1, 1, 100m, TransactionType.Expense, DateTime.UtcNow, null),
                CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }
}

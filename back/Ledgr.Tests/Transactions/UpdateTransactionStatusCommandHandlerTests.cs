using FluentAssertions;
using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Transactions.Commands.UpdateTransactionStatus;
using Ledgr.Domain.Entities;
using Ledgr.Domain.Enums;
using Ledgr.Infrastructure.Persistence;
using Ledgr.Tests.Helpers;

namespace Ledgr.Tests.Transactions;

public class UpdateTransactionStatusCommandHandlerTests
{
    private static async Task<LedgrDbContext> SeedAsync(TransactionStatus status = TransactionStatus.Pending)
    {
        var db = DbContextFactory.Create();
        db.Organizations.Add(new Organization { Id = 1, Name = "Org" });
        db.Accounts.Add(new Account { Id = 1, OrgId = 1, Name = "Main", Type = AccountType.Checking });
        db.Transactions.Add(new Transaction
        {
            Id = 1, OrgId = 1, AccountId = 1,
            Amount = 100m, Type = TransactionType.Expense,
            Status = status, Date = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
        return db;
    }

    [Theory]
    [InlineData(TransactionStatus.Confirmed)]
    [InlineData(TransactionStatus.Cancelled)]
    public async Task Handle_PendingToTerminal_Succeeds(TransactionStatus newStatus)
    {
        using var db = await SeedAsync();
        var handler = new UpdateTransactionStatusCommandHandler(db);

        await handler.Handle(new UpdateTransactionStatusCommand(1, 1, newStatus), CancellationToken.None);

        var tx = await db.Transactions.FindAsync(1);
        tx!.Status.Should().Be(newStatus);
    }

    [Fact]
    public async Task Handle_TransactionNotFound_ThrowsNotFound()
    {
        using var db = DbContextFactory.Create();
        var handler = new UpdateTransactionStatusCommandHandler(db);

        await handler.Invoking(h => h.Handle(
                new UpdateTransactionStatusCommand(999, 1, TransactionStatus.Confirmed),
                CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_TransactionFromOtherOrg_ThrowsForbidden()
    {
        using var db = await SeedAsync();
        var handler = new UpdateTransactionStatusCommandHandler(db);

        // tx.OrgId = 1, but we pass OrgId = 99
        await handler.Invoking(h => h.Handle(
                new UpdateTransactionStatusCommand(1, 99, TransactionStatus.Confirmed),
                CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }

    [Theory]
    [InlineData(TransactionStatus.Confirmed)]
    [InlineData(TransactionStatus.Cancelled)]
    public async Task Handle_NonPendingTransaction_ThrowsValidation(TransactionStatus existingStatus)
    {
        using var db = await SeedAsync(existingStatus);
        var handler = new UpdateTransactionStatusCommandHandler(db);

        await handler.Invoking(h => h.Handle(
                new UpdateTransactionStatusCommand(1, 1, TransactionStatus.Confirmed),
                CancellationToken.None))
            .Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_RevertToPending_ThrowsValidation()
    {
        using var db = await SeedAsync();
        var handler = new UpdateTransactionStatusCommandHandler(db);

        await handler.Invoking(h => h.Handle(
                new UpdateTransactionStatusCommand(1, 1, TransactionStatus.Pending),
                CancellationToken.None))
            .Should().ThrowAsync<ValidationException>();
    }
}

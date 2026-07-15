using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler(ILedgrDbContext db) : IRequestHandler<CreateTransactionCommand, int>
{
    public async Task<int> Handle(CreateTransactionCommand req, CancellationToken ct)
    {
        if (req.Amount <= 0)
            throw new ValidationException("Amount must be greater than zero.");

        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == req.AccountId, ct)
            ?? throw new NotFoundException("Account not found.");

        if (account.OrgId != req.OrgId)
            throw new ForbiddenException("Account does not belong to your organization.");

        if (req.CategoryId.HasValue)
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == req.CategoryId, ct)
                ?? throw new NotFoundException("Category not found.");
            if (category.OrgId != req.OrgId)
                throw new ForbiddenException("Category does not belong to your organization.");
        }

        var tx = new Transaction
        {
            OrgId       = req.OrgId,
            AccountId   = req.AccountId,
            CategoryId  = req.CategoryId,
            Amount      = req.Amount,
            Type        = req.Type,
            Date        = req.Date,
            Description = req.Description,
        };

        db.Transactions.Add(tx);
        await db.SaveChangesAsync(ct);
        return tx.Id;
    }
}

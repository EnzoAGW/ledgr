using Ledgr.Application.Common.Exceptions;
using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Transactions.Commands.UpdateTransactionStatus;

public class UpdateTransactionStatusCommandHandler(ILedgrDbContext db) : IRequestHandler<UpdateTransactionStatusCommand>
{
    public async Task Handle(UpdateTransactionStatusCommand req, CancellationToken ct)
    {
        var tx = await db.Transactions.FirstOrDefaultAsync(t => t.Id == req.TransactionId, ct)
            ?? throw new NotFoundException("Transaction not found.");

        if (tx.OrgId != req.OrgId)
            throw new ForbiddenException();

        if (tx.Status != TransactionStatus.Pending)
            throw new ValidationException("Only pending transactions can be updated.");

        if (req.NewStatus == TransactionStatus.Pending)
            throw new ValidationException("Cannot revert to pending.");

        tx.Status = req.NewStatus;
        await db.SaveChangesAsync(ct);
    }
}

using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand(
    int OrgId,
    int AccountId,
    int? CategoryId,
    decimal Amount,
    TransactionType Type,
    DateTime Date,
    string? Description
) : IRequest<int>;

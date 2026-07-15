using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Transactions.Commands.UpdateTransactionStatus;

public record UpdateTransactionStatusCommand(int TransactionId, int OrgId, TransactionStatus NewStatus) : IRequest;

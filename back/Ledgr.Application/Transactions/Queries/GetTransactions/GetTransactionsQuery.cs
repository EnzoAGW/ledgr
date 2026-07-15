using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Transactions.Queries.GetTransactions;

public record GetTransactionsQuery(
    int OrgId,
    string? Search,
    int? AccountId,
    int? CategoryId,
    TransactionType? Type,
    TransactionStatus? Status,
    DateTime? From,
    DateTime? To,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<TransactionDto>>;

public record TransactionDto(
    int Id,
    decimal Amount,
    TransactionType Type,
    TransactionStatus Status,
    DateTime Date,
    string? Description,
    int AccountId,
    string AccountName,
    int? CategoryId,
    string? CategoryName,
    string? CategoryColor
);

public record PagedResult<T>(IList<T> Items, int TotalCount, int Page, int PageSize);

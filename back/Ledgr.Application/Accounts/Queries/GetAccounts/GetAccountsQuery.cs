using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Accounts.Queries.GetAccounts;

public record GetAccountsQuery(int OrgId) : IRequest<IList<AccountDto>>;

public record AccountDto(int Id, string Name, AccountType Type, decimal Balance);

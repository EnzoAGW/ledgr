using Ledgr.Domain.Enums;
using MediatR;

namespace Ledgr.Application.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(int OrgId, string Name, AccountType Type) : IRequest<int>;

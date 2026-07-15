using Ledgr.Domain.Enums;

namespace Ledgr.Domain.Entities;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }

    public int OrgId { get; set; }
    public Organization Org { get; set; } = null!;

    public ICollection<Transaction> Transactions { get; set; } = [];
}

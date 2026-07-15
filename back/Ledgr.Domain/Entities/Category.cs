using Ledgr.Domain.Enums;

namespace Ledgr.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
    public string Color { get; set; } = "#6366F1";

    public int OrgId { get; set; }
    public Organization Org { get; set; } = null!;

    public ICollection<Transaction> Transactions { get; set; } = [];
}

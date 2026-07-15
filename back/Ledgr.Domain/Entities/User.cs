using Ledgr.Domain.Enums;

namespace Ledgr.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public int OrgId { get; set; }
    public Organization Org { get; set; } = null!;
}

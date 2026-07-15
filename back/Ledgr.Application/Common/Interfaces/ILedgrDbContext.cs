using Ledgr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Application.Common.Interfaces;

public interface ILedgrDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<User> Users { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Category> Categories { get; }
    DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

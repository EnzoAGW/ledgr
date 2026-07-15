using Ledgr.Application.Common.Interfaces;
using Ledgr.Domain.Entities;
using Ledgr.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Infrastructure.Persistence;

public class LedgrDbContext(DbContextOptions<LedgrDbContext> options) : DbContext(options), ILedgrDbContext
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User>         Users         => Set<User>();
    public DbSet<Account>      Accounts      => Set<Account>();
    public DbSet<Category>     Categories    => Set<Category>();
    public DbSet<Transaction>  Transactions  => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        b.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        b.Entity<Account>()
            .Property(a => a.Type)
            .HasConversion<string>();

        b.Entity<Category>()
            .Property(c => c.Type)
            .HasConversion<string>();

        b.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>();

        b.Entity<Transaction>()
            .Property(t => t.Status)
            .HasConversion<string>();

        b.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasColumnType("numeric(18,2)");
    }
}

using Ledgr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ledgr.Tests.Helpers;

internal static class DbContextFactory
{
    internal static LedgrDbContext Create() =>
        new(new DbContextOptionsBuilder<LedgrDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}

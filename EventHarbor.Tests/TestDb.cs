using EventHarbor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EventHarbor.Tests;

/// <summary>
/// Minimal IDbContextFactory backed by in-memory SQLite for service tests.
/// Each factory instance owns a fresh unique-named in-memory DB so tests are isolated.
/// </summary>
internal sealed class TestDbFactory : IDbContextFactory<EventHarborDbContext>, IDisposable
{
    private readonly DbContextOptions<EventHarborDbContext> _options;

    public TestDbFactory()
    {
        _options = new DbContextOptionsBuilder<EventHarborDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var ctx = new EventHarborDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    public EventHarborDbContext CreateDbContext() => new(_options);

    public void Dispose()
    {
        using var ctx = CreateDbContext();
        ctx.Database.EnsureDeleted();
    }
}

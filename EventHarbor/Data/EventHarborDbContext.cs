using EventHarbor.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventHarbor.Data;

public class EventHarborDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<CultureAction> CultureActions => Set<CultureAction>();

    public EventHarborDbContext() { }

    public EventHarborDbContext(DbContextOptions<EventHarborDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={AppPaths.DatabasePath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<CultureAction>()
            .HasIndex(c => new { c.OwnerId, c.StartAt });
    }
}

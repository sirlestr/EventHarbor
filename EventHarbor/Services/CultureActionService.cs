using EventHarbor.Data;
using EventHarbor.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventHarbor.Services;

public class CultureActionService : ICultureActionService
{
    private readonly IDbContextFactory<EventHarborDbContext> _factory;

    public CultureActionService(IDbContextFactory<EventHarborDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<CultureAction>> GetAllForOwnerAsync(int ownerId, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.CultureActions
            .AsNoTracking()
            .Where(a => a.OwnerId == ownerId)
            .OrderByDescending(a => a.StartAt)
            .ToListAsync(ct);
    }

    public async Task<CultureAction?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.CultureActions.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<CultureAction> CreateAsync(CultureAction action, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        action.CreatedAt = DateTime.UtcNow;
        action.UpdatedAt = action.CreatedAt;
        db.CultureActions.Add(action);
        await db.SaveChangesAsync(ct);
        return action;
    }

    public async Task UpdateAsync(CultureAction action, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        action.UpdatedAt = DateTime.UtcNow;
        db.CultureActions.Update(action);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        var entity = await db.CultureActions.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (entity is null) return;
        db.CultureActions.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}

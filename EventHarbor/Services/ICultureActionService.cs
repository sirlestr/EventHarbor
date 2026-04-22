using EventHarbor.Domain;

namespace EventHarbor.Services;

public interface ICultureActionService
{
    Task<IReadOnlyList<CultureAction>> GetAllForOwnerAsync(int ownerId, CancellationToken ct = default);
    Task<CultureAction?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CultureAction> CreateAsync(CultureAction action, CancellationToken ct = default);
    Task UpdateAsync(CultureAction action, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

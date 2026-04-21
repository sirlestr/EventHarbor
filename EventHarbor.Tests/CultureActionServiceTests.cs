using EventHarbor.Domain;
using EventHarbor.Services;

namespace EventHarbor.Tests;

public class CultureActionServiceTests
{
    private const int OwnerA = 1;
    private const int OwnerB = 2;

    private static CultureAction Sample(string name, int owner = OwnerA) => new()
    {
        Name = name,
        StartAt = new DateTime(2026, 5, 17, 18, 0, 0),
        EndAt = new DateTime(2026, 5, 17, 23, 30, 0),
        Children = 10,
        Adults = 40,
        Seniors = 5,
        Disabled = 2,
        Cost = 1234.56m,
        IsFree = false,
        Type = CultureEventType.Concert,
        Exhibition = ExhibitionType.Classic,
        Organiser = Organiser.Museum,
        OwnerId = owner,
    };

    [Fact]
    public async Task CreateAsync_assigns_identity_and_persists()
    {
        using var factory = new TestDbFactory();
        var svc = new CultureActionService(factory);

        var created = await svc.CreateAsync(Sample("Noc muzeí"));

        Assert.True(created.Id > 0);
        var fetched = await svc.GetByIdAsync(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Noc muzeí", fetched!.Name);
        Assert.Equal(57m, fetched.TotalAttendance);
    }

    [Fact]
    public async Task GetAllForOwner_filters_by_owner()
    {
        using var factory = new TestDbFactory();
        var svc = new CultureActionService(factory);

        await svc.CreateAsync(Sample("A1", OwnerA));
        await svc.CreateAsync(Sample("A2", OwnerA));
        await svc.CreateAsync(Sample("B1", OwnerB));

        var a = await svc.GetAllForOwnerAsync(OwnerA);
        var b = await svc.GetAllForOwnerAsync(OwnerB);

        Assert.Equal(2, a.Count);
        Assert.Single(b);
        Assert.Equal("B1", b[0].Name);
    }

    [Fact]
    public async Task UpdateAsync_persists_changes()
    {
        using var factory = new TestDbFactory();
        var svc = new CultureActionService(factory);

        var created = await svc.CreateAsync(Sample("Original"));
        created.Name = "Upraveno";
        created.Adults = 999;
        await svc.UpdateAsync(created);

        var reloaded = await svc.GetByIdAsync(created.Id);
        Assert.Equal("Upraveno", reloaded!.Name);
        Assert.Equal(999, reloaded.Adults);
    }

    [Fact]
    public async Task DeleteAsync_removes_record()
    {
        using var factory = new TestDbFactory();
        var svc = new CultureActionService(factory);

        var a = await svc.CreateAsync(Sample("To delete"));

        await svc.DeleteAsync(a.Id);

        Assert.Null(await svc.GetByIdAsync(a.Id));
    }

    [Fact]
    public async Task DeleteAsync_is_noop_for_missing_id()
    {
        using var factory = new TestDbFactory();
        var svc = new CultureActionService(factory);

        await svc.DeleteAsync(9999);
        // no exception == pass
    }
}

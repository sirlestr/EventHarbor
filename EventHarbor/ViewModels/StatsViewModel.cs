using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EventHarbor.Domain;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public class MonthBar
{
    public const double ChartHeight = 160.0;

    public string Label { get; init; } = string.Empty;
    public int Children { get; init; }
    public int Adults { get; init; }
    public int Seniors { get; init; }
    public int Disabled { get; init; }
    public int Total => Children + Adults + Seniors + Disabled;

    public double StackHeight { get; set; }     // px
    public double ChildrenPx { get; set; }
    public double AdultsPx { get; set; }
    public double SeniorsPx { get; set; }
    public double DisabledPx { get; set; }
}

public class OrganiserSlice
{
    public string Name { get; init; } = string.Empty;
    public int Count { get; init; }
    public int Visitors { get; init; }
    public string ColorHex { get; init; } = "#1E3A5F";
    public double StartFraction { get; set; }   // 0..1
    public double EndFraction { get; set; }
}

public class TopEntry
{
    public int Rank { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Total { get; init; }
    public double FillPct { get; set; }
}

public partial class StatsViewModel : ObservableObject
{
    private readonly ICultureActionService _service;
    private readonly SessionState _session;

    [ObservableProperty] private int _totalVisitors;
    [ObservableProperty] private int _doneEvents;
    [ObservableProperty] private decimal _totalCost;
    [ObservableProperty] private int _freeCount;
    [ObservableProperty] private int _totalEvents;
    [ObservableProperty] private string _freeShareLabel = "0 %";

    public ObservableCollection<MonthBar> MonthBars { get; } = new();
    public ObservableCollection<OrganiserSlice> OrganiserSlices { get; } = new();
    public ObservableCollection<TopEntry> TopEvents { get; } = new();

    [ObservableProperty] private int _organiserTotalVisitors;

    public StatsViewModel(ICultureActionService service, SessionState session)
    {
        _service = service;
        _session = session;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var items = await _service.GetAllForOwnerAsync(_session.UserId);

        TotalEvents = items.Count;
        TotalVisitors = items.Sum(e => e.TotalAttendance);
        TotalCost = items.Where(e => !e.IsFree).Sum(e => e.Cost);
        FreeCount = items.Count(e => e.IsFree);
        DoneEvents = items.Count(e => e.Status == EventStatus.Ended);
        FreeShareLabel = TotalEvents > 0
            ? $"{(int)Math.Round(FreeCount * 100.0 / TotalEvents)} % programu"
            : "—";

        BuildMonthBars(items);
        BuildOrganiserSlices(items);
        BuildTopEvents(items);
    }

    private void BuildMonthBars(IReadOnlyList<CultureAction> items)
    {
        MonthBars.Clear();
        var dated = items.Where(e => e.StartAt.HasValue).ToList();
        if (dated.Count == 0) return;

        var now = DateTime.Now;
        var bars = new List<MonthBar>();
        for (int offset = -6; offset <= 0; offset++)
        {
            var anchor = new DateTime(now.Year, now.Month, 1).AddMonths(offset);
            var next = anchor.AddMonths(1);
            var inMonth = dated.Where(e => e.StartAt!.Value >= anchor && e.StartAt.Value < next).ToList();
            bars.Add(new MonthBar
            {
                Label = anchor.ToString("MMM", System.Globalization.CultureInfo.GetCultureInfo("cs-CZ")).TrimEnd('.'),
                Children = inMonth.Sum(e => e.Children),
                Adults = inMonth.Sum(e => e.Adults),
                Seniors = inMonth.Sum(e => e.Seniors),
                Disabled = inMonth.Sum(e => e.Disabled),
            });
        }

        var maxMonth = Math.Max(1, bars.Max(b => b.Total));
        foreach (var b in bars)
        {
            b.StackHeight = (double)b.Total / maxMonth * MonthBar.ChartHeight;
            if (b.Total > 0)
            {
                b.ChildrenPx = b.StackHeight * b.Children / b.Total;
                b.AdultsPx = b.StackHeight * b.Adults / b.Total;
                b.SeniorsPx = b.StackHeight * b.Seniors / b.Total;
                b.DisabledPx = b.StackHeight * b.Disabled / b.Total;
            }
            MonthBars.Add(b);
        }
    }

    private void BuildOrganiserSlices(IReadOnlyList<CultureAction> items)
    {
        OrganiserSlices.Clear();
        var defs = new[]
        {
            (Name: "Muzeum", Key: Organiser.Museum, Color: "#1E3A5F"),
            (Name: "Město", Key: Organiser.City, Color: "#B85C1F"),
            (Name: "Ostatní", Key: Organiser.Other, Color: "#4A6B3D"),
        };

        var slices = defs
            .Select(d => new OrganiserSlice
            {
                Name = d.Name,
                ColorHex = d.Color,
                Count = items.Count(e => e.Organiser == d.Key),
                Visitors = items.Where(e => e.Organiser == d.Key).Sum(e => e.TotalAttendance),
            })
            .ToList();

        OrganiserTotalVisitors = slices.Sum(s => s.Visitors);

        double acc = 0;
        foreach (var s in slices)
        {
            var frac = OrganiserTotalVisitors == 0 ? 0 : (double)s.Visitors / OrganiserTotalVisitors;
            s.StartFraction = acc;
            s.EndFraction = acc + frac;
            acc = s.EndFraction;
            OrganiserSlices.Add(s);
        }
    }

    private void BuildTopEvents(IReadOnlyList<CultureAction> items)
    {
        TopEvents.Clear();
        var top = items
            .Where(e => e.TotalAttendance > 0)
            .OrderByDescending(e => e.TotalAttendance)
            .Take(5)
            .ToList();

        if (top.Count == 0) return;
        var max = Math.Max(1, top[0].TotalAttendance);

        int rank = 1;
        foreach (var e in top)
        {
            TopEvents.Add(new TopEntry
            {
                Rank = rank++,
                Name = e.Name,
                Total = e.TotalAttendance,
                FillPct = (double)e.TotalAttendance / max,
            });
        }
    }
}

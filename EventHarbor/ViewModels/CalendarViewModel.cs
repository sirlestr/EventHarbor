using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHarbor.Domain;

namespace EventHarbor.ViewModels;

public class CalendarDay
{
    public DateTime Date { get; init; }
    public int DayNumber => Date.Day;
    public bool IsOutOfMonth { get; init; }
    public bool IsToday { get; init; }
    public List<CultureAction> Events { get; init; } = new();
    public int ExtraEventsCount => Math.Max(0, Events.Count - 3);
    public bool HasExtras => Events.Count > 3;
    public IEnumerable<CultureAction> VisibleEvents => Events.Take(3);
}

public partial class CalendarViewModel : ObservableObject
{
    private readonly Func<IEnumerable<CultureAction>> _sourceProvider;
    private readonly Action<CultureAction>? _onPick;

    [ObservableProperty]
    private DateTime _viewMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);

    public ObservableCollection<CalendarDay> Days { get; } = new();

    public string MonthLabel =>
        CultureInfo.GetCultureInfo("cs-CZ").TextInfo.ToTitleCase(
            ViewMonth.ToString("LLLL yyyy", CultureInfo.GetCultureInfo("cs-CZ")));

    public IReadOnlyList<string> DayHeaders { get; } = new[] { "Po", "Út", "St", "Čt", "Pá", "So", "Ne" };

    public CalendarViewModel(Func<IEnumerable<CultureAction>> sourceProvider, Action<CultureAction>? onPick = null)
    {
        _sourceProvider = sourceProvider;
        _onPick = onPick;
        Rebuild();
    }

    public void Refresh() => Rebuild();

    partial void OnViewMonthChanged(DateTime value)
    {
        OnPropertyChanged(nameof(MonthLabel));
        Rebuild();
    }

    [RelayCommand]
    private void PreviousMonth() => ViewMonth = ViewMonth.AddMonths(-1);

    [RelayCommand]
    private void NextMonth() => ViewMonth = ViewMonth.AddMonths(1);

    [RelayCommand]
    private void Today() => ViewMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

    [RelayCommand]
    private void Pick(CultureAction? action)
    {
        if (action is not null) _onPick?.Invoke(action);
    }

    private void Rebuild()
    {
        Days.Clear();
        var year = ViewMonth.Year;
        var month = ViewMonth.Month;
        var first = new DateTime(year, month, 1);
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var today = DateTime.Today;
        // Monday-first week: DayOfWeek Monday=1..Sunday=0 -> normalize
        var startDow = ((int)first.DayOfWeek + 6) % 7;

        var events = _sourceProvider().ToList();

        // leading days from previous month
        for (int i = 0; i < startDow; i++)
        {
            var d = first.AddDays(-(startDow - i));
            Days.Add(MakeDay(d, isOutOfMonth: true, today, events));
        }
        // days in current month
        for (int i = 1; i <= daysInMonth; i++)
        {
            var d = new DateTime(year, month, i);
            Days.Add(MakeDay(d, isOutOfMonth: false, today, events));
        }
        // trailing days until week ends
        while (Days.Count % 7 != 0)
        {
            var last = Days[^1].Date;
            Days.Add(MakeDay(last.AddDays(1), isOutOfMonth: true, today, events));
        }
    }

    private static CalendarDay MakeDay(DateTime date, bool isOutOfMonth, DateTime today, IReadOnlyList<CultureAction> events)
    {
        var dayStart = date.Date;
        var dayEnd = date.Date.AddDays(1);
        var matches = events
            .Where(e => e.StartAt.HasValue && e.EndAt.HasValue
                        && e.StartAt.Value < dayEnd
                        && e.EndAt.Value >= dayStart)
            .ToList();

        return new CalendarDay
        {
            Date = date,
            IsOutOfMonth = isOutOfMonth,
            IsToday = date.Date == today && !isOutOfMonth,
            Events = matches,
        };
    }
}

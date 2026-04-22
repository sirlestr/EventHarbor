using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHarbor.Domain;
using EventHarbor.Services;
using EventHarbor.Views;

namespace EventHarbor.ViewModels;

public enum ListViewMode
{
    Table,
    Cards,
    Calendar,
}

public partial class ListViewModel : ObservableObject
{
    private readonly ICultureActionService _service;
    private readonly SessionState _session;

    public event EventHandler? NewRequested;
    public event EventHandler<CultureAction>? EditRequested;

    public ObservableCollection<CultureAction> Events { get; } = new();
    public ICollectionView EventsView { get; }

    public CalendarViewModel Calendar { get; }

    [ObservableProperty]
    private CultureAction? _selectedEvent;

    [ObservableProperty]
    private ListViewMode _viewMode = ListViewMode.Table;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private Organiser? _filterOrganiser;

    [ObservableProperty]
    private EventStatus? _filterStatus;

    [ObservableProperty]
    private CultureEventType? _filterType;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _filteredCount;

    [ObservableProperty]
    private int _plannedCount;

    [ObservableProperty]
    private int _runningCount;

    [ObservableProperty]
    private int _endedCount;

    public ListViewModel(ICultureActionService service, SessionState session)
    {
        _service = service;
        _session = session;

        EventsView = CollectionViewSource.GetDefaultView(Events);
        EventsView.Filter = FilterEvent;

        Calendar = new CalendarViewModel(
            () => EventsView.Cast<CultureAction>(),
            a => SelectedEvent = a);

        _ = LoadAsync();
    }

    public async Task LoadAsync()
    {
        Events.Clear();
        var items = await _service.GetAllForOwnerAsync(_session.UserId);
        foreach (var e in items) Events.Add(e);
        TotalCount = Events.Count;
        PlannedCount = Events.Count(e => e.Status == EventStatus.Planned);
        RunningCount = Events.Count(e => e.Status == EventStatus.Running);
        EndedCount = Events.Count(e => e.Status == EventStatus.Ended);
        RefreshFilter();
        if (SelectedEvent is null || !Events.Contains(SelectedEvent))
            SelectedEvent = Events.FirstOrDefault();
    }

    private bool FilterEvent(object item)
    {
        if (item is not CultureAction a) return false;
        if (!string.IsNullOrWhiteSpace(SearchText) &&
            (a.Name is null || !a.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            return false;
        if (FilterOrganiser.HasValue && a.Organiser != FilterOrganiser.Value) return false;
        if (FilterStatus.HasValue && a.Status != FilterStatus.Value) return false;
        if (FilterType.HasValue && a.Type != FilterType.Value) return false;
        return true;
    }

    partial void OnSearchTextChanged(string value) => RefreshFilter();
    partial void OnFilterOrganiserChanged(Organiser? value) => RefreshFilter();
    partial void OnFilterStatusChanged(EventStatus? value) => RefreshFilter();
    partial void OnFilterTypeChanged(CultureEventType? value) => RefreshFilter();
    partial void OnViewModeChanged(ListViewMode value) { /* view switching hook */ }

    private void RefreshFilter()
    {
        EventsView.Refresh();
        FilteredCount = EventsView.Cast<object>().Count();
        Calendar.Refresh();
    }

    [RelayCommand]
    private void ToggleOrganiser(string org)
    {
        if (!Enum.TryParse<Organiser>(org, out var parsed)) return;
        FilterOrganiser = FilterOrganiser == parsed ? null : parsed;
    }

    [RelayCommand]
    private void ToggleStatus(string status)
    {
        if (!Enum.TryParse<EventStatus>(status, out var parsed)) return;
        FilterStatus = FilterStatus == parsed ? null : parsed;
    }

    [RelayCommand]
    private void ToggleType(string type)
    {
        if (!Enum.TryParse<CultureEventType>(type, out var parsed)) return;
        FilterType = FilterType == parsed ? null : parsed;
    }

    [RelayCommand]
    private void ClearFilters()
    {
        FilterOrganiser = null;
        FilterStatus = null;
        FilterType = null;
        SearchText = string.Empty;
    }

    [RelayCommand]
    private void SelectCard(CultureAction? action)
    {
        if (action is not null) SelectedEvent = action;
    }

    [RelayCommand]
    private void SetViewMode(string mode)
    {
        if (Enum.TryParse<ListViewMode>(mode, out var parsed)) ViewMode = parsed;
    }

    [RelayCommand]
    private void New() => NewRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private void Edit()
    {
        if (SelectedEvent is not null)
            EditRequested?.Invoke(this, SelectedEvent);
    }

    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private async Task DeleteAsync()
    {
        if (SelectedEvent is null) return;

        var confirmed = ConfirmDialog.Ask(
            "Smazat akci?",
            $"Opravdu chcete smazat akci „{SelectedEvent.Name}\"? Tuto operaci nelze vzít zpět.",
            confirmText: "Smazat",
            cancelText: "Zrušit",
            iconKind: "trash",
            destructive: true);

        if (!confirmed) return;

        var id = SelectedEvent.Id;
        await _service.DeleteAsync(id);
        var idx = Events.IndexOf(SelectedEvent);
        Events.Remove(SelectedEvent);
        TotalCount = Events.Count;
        RefreshFilter();
        SelectedEvent = Events.Count > 0
            ? Events[Math.Min(idx, Events.Count - 1)]
            : null;
    }

    private bool CanEditOrDelete() => SelectedEvent is not null;

    partial void OnSelectedEventChanged(CultureAction? value)
    {
        EditCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}

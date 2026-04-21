using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHarbor.Domain;
using EventHarbor.Services;

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
    private int _totalCount;

    [ObservableProperty]
    private int _filteredCount;

    public ListViewModel(ICultureActionService service, SessionState session)
    {
        _service = service;
        _session = session;

        EventsView = CollectionViewSource.GetDefaultView(Events);
        EventsView.Filter = FilterEvent;

        _ = LoadAsync();
    }

    public async Task LoadAsync()
    {
        Events.Clear();
        var items = await _service.GetAllForOwnerAsync(_session.UserId);
        foreach (var e in items) Events.Add(e);
        TotalCount = Events.Count;
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
        return true;
    }

    partial void OnSearchTextChanged(string value) => RefreshFilter();
    partial void OnFilterOrganiserChanged(Organiser? value) => RefreshFilter();
    partial void OnFilterStatusChanged(EventStatus? value) => RefreshFilter();
    partial void OnViewModeChanged(ListViewMode value) { /* view switching hook */ }

    private void RefreshFilter()
    {
        EventsView.Refresh();
        FilteredCount = EventsView.Cast<object>().Count();
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

        var result = MessageBox.Show(
            $"Opravdu chcete smazat akci \"{SelectedEvent.Name}\"?",
            "Potvrzení smazání",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

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

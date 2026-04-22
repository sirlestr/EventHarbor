using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EventHarbor.Domain;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public enum MainRoute
{
    List,
    Form,
    Stats,
}

public partial class MainShellViewModel : ObservableObject
{
    private readonly Func<ListViewModel> _listFactory;
    private readonly Func<FormViewModel> _formFactory;
    private readonly Func<StatsViewModel> _statsFactory;
    private readonly SessionState _session;
    private readonly ThemeManager _theme;

    [ObservableProperty]
    private MainRoute _currentRoute = MainRoute.List;

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ThemeLabel))]
    private AppTheme _currentTheme;

    [ObservableProperty]
    private int _plannedCount;

    [ObservableProperty]
    private int _runningCount;

    [ObservableProperty]
    private int _endedCount;

    [ObservableProperty]
    private int _totalCount;

    private ListViewModel? _currentList;

    public string LoggedUserName => _session.UserName;

    public string ThemeLabel => CurrentTheme == AppTheme.Dark ? "Světlé" : "Tmavé";

    public string UserInitials
    {
        get
        {
            var n = _session.UserName.Trim();
            if (string.IsNullOrEmpty(n)) return "?";
            var parts = n.Split(new[] { ' ', '.', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return (parts[0][0].ToString() + parts[1][0].ToString()).ToUpperInvariant();
            return n.Length >= 2 ? n[..2].ToUpperInvariant() : n.ToUpperInvariant();
        }
    }

    public MainShellViewModel(
        Func<ListViewModel> listFactory,
        Func<FormViewModel> formFactory,
        Func<StatsViewModel> statsFactory,
        SessionState session,
        ThemeManager theme)
    {
        _listFactory = listFactory;
        _formFactory = formFactory;
        _statsFactory = statsFactory;
        _session = session;
        _theme = theme;
        CurrentTheme = theme.Current;
        GoToCommand.Execute(nameof(MainRoute.List));
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        _theme.Toggle();
        CurrentTheme = _theme.Current;
    }

    [RelayCommand]
    private void GoTo(string? route)
    {
        if (!Enum.TryParse<MainRoute>(route, out var parsed))
            parsed = MainRoute.List;

        CurrentRoute = parsed;
        CurrentViewModel = parsed switch
        {
            MainRoute.Form => _formFactory(),
            MainRoute.Stats => _statsFactory(),
            _ => GetOrCreateList(),
        };
    }

    private ListViewModel GetOrCreateList()
    {
        var vm = _listFactory();
        if (_currentList is not null)
            _currentList.PropertyChanged -= OnListPropertyChanged;
        _currentList = vm;
        _currentList.PropertyChanged += OnListPropertyChanged;
        SyncFromList();
        return vm;
    }

    private void OnListPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ListViewModel.PlannedCount)
            or nameof(ListViewModel.RunningCount)
            or nameof(ListViewModel.EndedCount)
            or nameof(ListViewModel.TotalCount))
        {
            SyncFromList();
        }
    }

    private void SyncFromList()
    {
        if (_currentList is null) return;
        PlannedCount = _currentList.PlannedCount;
        RunningCount = _currentList.RunningCount;
        EndedCount = _currentList.EndedCount;
        TotalCount = _currentList.TotalCount;
    }

    [RelayCommand]
    private void FilterByStatus(string status)
    {
        if (!Enum.TryParse<EventStatus>(status, out var parsed)) return;
        EnsureListActive();
        if (_currentList is not null)
            _currentList.FilterStatus = _currentList.FilterStatus == parsed ? null : parsed;
    }

    [RelayCommand]
    private void FilterByOrganiser(string org)
    {
        if (!Enum.TryParse<Organiser>(org, out var parsed)) return;
        EnsureListActive();
        if (_currentList is not null)
            _currentList.FilterOrganiser = _currentList.FilterOrganiser == parsed ? null : parsed;
    }

    [RelayCommand]
    private void FilterByType(string type)
    {
        if (!Enum.TryParse<CultureEventType>(type, out var parsed)) return;
        EnsureListActive();
        if (_currentList is not null)
            _currentList.FilterType = _currentList.FilterType == parsed ? null : parsed;
    }

    [RelayCommand]
    private void ClearAllFilters()
    {
        EnsureListActive();
        if (_currentList is not null)
        {
            _currentList.FilterStatus = null;
            _currentList.FilterOrganiser = null;
            _currentList.FilterType = null;
            _currentList.SearchText = string.Empty;
        }
    }

    private void EnsureListActive()
    {
        if (CurrentRoute != MainRoute.List)
        {
            CurrentRoute = MainRoute.List;
            CurrentViewModel = GetOrCreateList();
        }
    }

    public void StartNewEvent()
    {
        var vm = _formFactory();
        vm.Initialize(null);
        CurrentRoute = MainRoute.Form;
        CurrentViewModel = vm;
    }

    [RelayCommand]
    private void StartNewFromSidebar() => StartNewEvent();

    [RelayCommand]
    private void FocusSearch()
    {
        EnsureListActive();
        WeakReferenceMessenger.Default.Send(new FocusSearchMessage());
    }

    public void StartEditEvent(CultureAction action)
    {
        var vm = _formFactory();
        vm.Initialize(action);
        CurrentRoute = MainRoute.Form;
        CurrentViewModel = vm;
    }

    public void ReturnToList()
    {
        CurrentRoute = MainRoute.List;
        if (_currentList is not null)
        {
            // Reuse the list instance from which the user opened the form
            // so ViewMode (Table/Cards/Calendar) and active filters are preserved.
            CurrentViewModel = _currentList;
            _ = _currentList.LoadAsync();
        }
        else
        {
            CurrentViewModel = GetOrCreateList();
        }
    }
}

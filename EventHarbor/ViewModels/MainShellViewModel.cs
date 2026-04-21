using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            _ => _listFactory(),
        };
    }

    public void StartNewEvent()
    {
        var vm = _formFactory();
        vm.Initialize(null);
        CurrentRoute = MainRoute.Form;
        CurrentViewModel = vm;
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
        CurrentViewModel = _listFactory();
    }
}

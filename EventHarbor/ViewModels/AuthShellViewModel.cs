using CommunityToolkit.Mvvm.ComponentModel;

namespace EventHarbor.ViewModels;

public enum AuthScreen
{
    Login,
    Register,
    Forgot,
}

public partial class AuthShellViewModel : ObservableObject
{
    private readonly Func<LoginViewModel> _loginFactory;
    private readonly Func<RegisterViewModel> _registerFactory;
    private readonly Func<ForgotViewModel> _forgotFactory;

    [ObservableProperty]
    private AuthScreen _currentScreen = AuthScreen.Login;

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    public event EventHandler? AuthSucceeded;

    public AuthShellViewModel(
        Func<LoginViewModel> loginFactory,
        Func<RegisterViewModel> registerFactory,
        Func<ForgotViewModel> forgotFactory)
    {
        _loginFactory = loginFactory;
        _registerFactory = registerFactory;
        _forgotFactory = forgotFactory;
        CurrentViewModel = _loginFactory();
    }

    public void GoTo(AuthScreen screen)
    {
        CurrentScreen = screen;
        CurrentViewModel = screen switch
        {
            AuthScreen.Register => _registerFactory(),
            AuthScreen.Forgot => _forgotFactory(),
            _ => _loginFactory(),
        };
    }

    public void RaiseAuthSucceeded() => AuthSucceeded?.Invoke(this, EventArgs.Empty);
}

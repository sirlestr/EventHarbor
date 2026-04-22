using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly RememberMeService _rememberMeStore;
    private readonly SessionState _session;

    public event EventHandler? LoginSucceeded;
    public event EventHandler? NavigateToRegister;
    public event EventHandler? NavigateToForgot;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _rememberMe = true;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    public LoginViewModel(IUserService userService, RememberMeService rememberMeStore, SessionState session)
    {
        _userService = userService;
        _rememberMeStore = rememberMeStore;
        _session = session;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = null;
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Zadejte uživatelské jméno a heslo.";
            return;
        }

        try
        {
            IsBusy = true;
            var result = await _userService.LoginAsync(UserName.Trim(), Password);
            switch (result)
            {
                case LoginResult.UserNotFound:
                    ErrorMessage = "Uživatel nenalezen.";
                    break;
                case LoginResult.WrongPassword:
                    ErrorMessage = "Špatné heslo, zkuste to prosím znovu.";
                    break;
                case LoginResult.Locked:
                    {
                        var secs = UserService.GetRemainingLockSeconds(UserName.Trim());
                        ErrorMessage = secs > 0
                            ? $"Příliš mnoho pokusů. Zkuste znovu za {secs}s."
                            : "Příliš mnoho pokusů. Zkuste znovu za okamžik.";
                    }
                    break;
                case LoginResult.Success:
                    if (RememberMe && _session.UserId > 0)
                        _rememberMeStore.Save(_session.UserId);
                    else
                        _rememberMeStore.Clear();
                    LoginSucceeded?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void GoToRegister() => NavigateToRegister?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void GoToForgot() => NavigateToForgot?.Invoke(this, EventArgs.Empty);
}

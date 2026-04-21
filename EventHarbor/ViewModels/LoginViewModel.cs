using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly AuthShellViewModel _shell;

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

    public LoginViewModel(IUserService userService, AuthShellViewModel shell)
    {
        _userService = userService;
        _shell = shell;
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
                case LoginResult.Success:
                    _shell.RaiseAuthSucceeded();
                    break;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void GoToRegister() => _shell.GoTo(AuthScreen.Register);

    [RelayCommand]
    private void GoToForgot() => _shell.GoTo(AuthScreen.Forgot);
}

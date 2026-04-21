using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly AuthShellViewModel _shell;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PasswordStrength))]
    [NotifyPropertyChangedFor(nameof(PasswordStrengthLabel))]
    [NotifyPropertyChangedFor(nameof(PasswordsMatch))]
    [NotifyPropertyChangedFor(nameof(PasswordsMismatch))]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PasswordsMatch))]
    [NotifyPropertyChangedFor(nameof(PasswordsMismatch))]
    private string _passwordConfirm = string.Empty;

    [ObservableProperty]
    private string _securityQuestion = Services.SecurityQuestions.All[0];

    [ObservableProperty]
    private string _securityAnswer = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    public IReadOnlyList<string> AvailableQuestions => Services.SecurityQuestions.All;

    public RegisterViewModel(IUserService userService, AuthShellViewModel shell)
    {
        _userService = userService;
        _shell = shell;
    }

    public int PasswordStrength
    {
        get
        {
            var p = Password;
            int score = 0;
            if (p.Length >= 8) score++;
            if (p.Any(char.IsUpper)) score++;
            if (p.Any(char.IsDigit)) score++;
            if (p.Any(c => !char.IsLetterOrDigit(c))) score++;
            return score;
        }
    }

    public string PasswordStrengthLabel => PasswordStrength switch
    {
        0 or 1 => "Slabé",
        2 => "Střední",
        3 => "Dobré",
        _ => "Silné",
    };

    public bool PasswordsMatch => !string.IsNullOrEmpty(PasswordConfirm) && Password == PasswordConfirm;
    public bool PasswordsMismatch => !string.IsNullOrEmpty(PasswordConfirm) && Password != PasswordConfirm;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(UserName) || UserName.Trim().Length < 3 || UserName.Trim().Length > 32)
        {
            ErrorMessage = "Uživatelské jméno musí mít 3–32 znaků.";
            return;
        }

        if (Password.Length < 4)
        {
            ErrorMessage = "Heslo musí mít alespoň 4 znaky.";
            return;
        }

        if (!PasswordsMatch)
        {
            ErrorMessage = "Hesla se neshodují.";
            return;
        }

        if (string.IsNullOrWhiteSpace(SecurityAnswer))
        {
            ErrorMessage = "Zadejte odpověď na bezpečnostní otázku.";
            return;
        }

        try
        {
            IsBusy = true;
            var ok = await _userService.RegisterAsync(UserName.Trim(), Password, SecurityQuestion, SecurityAnswer);
            if (!ok)
            {
                ErrorMessage = "Uživatel s tímto jménem už existuje.";
                return;
            }
            _shell.GoTo(AuthScreen.Login);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void GoToLogin() => _shell.GoTo(AuthScreen.Login);
}

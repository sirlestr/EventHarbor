using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHarbor.Services;

namespace EventHarbor.ViewModels;

public partial class ForgotViewModel : ObservableObject
{
    private const int VerificationValidityMinutes = 5;

    private readonly IUserService _userService;
    private DateTime? _verifiedAt;

    public event EventHandler? NavigateToLogin;
    public event EventHandler? ResetCompleted;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _securityAnswer = string.Empty;

    [ObservableProperty]
    private string _securityQuestion = Services.SecurityQuestions.All[0];

    [ObservableProperty]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    private string _newPasswordConfirm = string.Empty;

    [ObservableProperty]
    private bool _isVerified;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isBusy;

    public IReadOnlyList<string> AvailableQuestions => Services.SecurityQuestions.All;

    public ForgotViewModel(IUserService userService)
    {
        _userService = userService;
    }

    [RelayCommand]
    private async Task VerifyAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(SecurityAnswer))
        {
            ErrorMessage = "Vyplňte uživatelské jméno i odpověď.";
            return;
        }

        try
        {
            IsBusy = true;
            var ok = await _userService.VerifySecurityAnswerAsync(UserName.Trim(), SecurityAnswer);
            if (!ok)
            {
                ErrorMessage = "Uživatel nebyl nalezen nebo odpověď není správná.";
                return;
            }
            IsVerified = true;
            _verifiedAt = DateTime.UtcNow;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        ErrorMessage = null;

        if (!IsVerified || _verifiedAt is null ||
            DateTime.UtcNow - _verifiedAt.Value > TimeSpan.FromMinutes(VerificationValidityMinutes))
        {
            ErrorMessage = "Ověření vypršelo, ověřte odpověď znovu.";
            IsVerified = false;
            return;
        }

        if (NewPassword.Length < 4)
        {
            ErrorMessage = "Nové heslo musí mít alespoň 4 znaky.";
            return;
        }

        if (NewPassword != NewPasswordConfirm)
        {
            ErrorMessage = "Hesla se neshodují.";
            return;
        }

        try
        {
            IsBusy = true;
            var ok = await _userService.ResetPasswordAsync(UserName.Trim(), NewPassword);
            if (!ok)
            {
                ErrorMessage = "Reset se nepovedl.";
                return;
            }
            ResetCompleted?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void GoToLogin() => NavigateToLogin?.Invoke(this, EventArgs.Empty);
}

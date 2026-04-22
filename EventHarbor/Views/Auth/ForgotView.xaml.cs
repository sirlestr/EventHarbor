using System.Windows.Controls;
using EventHarbor.ViewModels;

namespace EventHarbor.Views.Auth;

public partial class ForgotView : UserControl
{
    public ForgotView()
    {
        InitializeComponent();
    }

    private void NewPwdBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ForgotViewModel vm)
            vm.NewPassword = ((PasswordBox)sender).Password;
    }

    private void NewPwdConfirmBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ForgotViewModel vm)
            vm.NewPasswordConfirm = ((PasswordBox)sender).Password;
    }
}

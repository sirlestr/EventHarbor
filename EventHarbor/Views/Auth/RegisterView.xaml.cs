using System.Windows.Controls;
using EventHarbor.ViewModels;

namespace EventHarbor.Views.Auth;

public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
    }

    private void PwdBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }

    private void PwdConfirmBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel vm)
            vm.PasswordConfirm = ((PasswordBox)sender).Password;
    }
}

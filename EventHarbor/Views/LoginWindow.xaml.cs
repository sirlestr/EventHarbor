using System.Windows;
using EventHarbor.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EventHarbor.Views;

public partial class LoginWindow : Window
{
    private readonly AuthShellViewModel _shell;

    public LoginWindow(AuthShellViewModel shell)
    {
        _shell = shell;
        DataContext = shell;
        InitializeComponent();

        shell.AuthSucceeded += OnAuthSucceeded;
    }

    private void OnAuthSucceeded(object? sender, EventArgs e)
    {
        _shell.AuthSucceeded -= OnAuthSucceeded;
        var main = App.AppHost.Services.GetRequiredService<MainWindow>();
        main.Show();
        Close();
    }
}

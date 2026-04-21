using System.Windows;
using EventHarbor.ViewModels;

namespace EventHarbor.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainShellViewModel shell)
    {
        DataContext = shell;
        InitializeComponent();
    }
}

using System.Windows;
using System.Windows.Input;

namespace EventHarbor.Views;

public partial class ConfirmDialog : Window
{
    public bool Confirmed { get; private set; }

    private ConfirmDialog()
    {
        InitializeComponent();

        MouseLeftButtonDown += (_, e) =>
        {
            if (e.ChangedButton == MouseButton.Left)
                try { DragMove(); } catch { }
        };
    }

    public static bool Ask(string title, string message,
                           string confirmText = "Potvrdit",
                           string cancelText = "Zrušit",
                           string iconKind = "bell",
                           bool destructive = false)
    {
        var dlg = new ConfirmDialog
        {
            TitleText = { Text = title },
            MessageText = { Text = message },
            ConfirmButton = { Content = confirmText },
            CancelButton = { Content = cancelText },
        };
        dlg.IconControl.Kind = iconKind;

        if (destructive)
        {
            dlg.ConfirmButton.Background = (System.Windows.Media.Brush)Application.Current.Resources["CopperBrush"];
        }

        dlg.Owner = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive) ?? Application.Current.MainWindow;

        dlg.ShowDialog();
        return dlg.Confirmed;
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = false;
        Close();
    }
}

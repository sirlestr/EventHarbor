using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using EventHarbor.ViewModels;

namespace EventHarbor.Views.List;

public partial class ListView : UserControl
{
    public ListView()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            WeakReferenceMessenger.Default.Register<ListView, FocusSearchMessage>(this,
                (recipient, _) =>
                {
                    recipient.SearchBox.Focus();
                    recipient.SearchBox.SelectAll();
                });
        };

        Unloaded += (_, _) =>
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        };
    }
}

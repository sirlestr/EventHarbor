using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EventHarbor.Common;

public class BorderlessWindow : Window
{
    public BorderlessWindow()
    {
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = System.Windows.Media.Brushes.Transparent;
        ResizeMode = ResizeMode.CanResize;
    }

    protected void EnableDragMove(FrameworkElement dragHandle)
    {
        dragHandle.MouseLeftButtonDown += (_, _) =>
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        };
    }
}

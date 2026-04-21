using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EventHarbor.Common;

/// <summary>
/// Registers class-level handlers on TextBox so that:
/// - Tab-focus (keyboard) selects all text.
/// - First mouse click on an unfocused TextBox also selects all (mimics browser/Windows common UX).
/// - Subsequent clicks inside an already-focused TextBox behave normally (cursor placement, range select).
/// </summary>
public static class TextBoxSelectAllBehavior
{
    public static void Register()
    {
        EventManager.RegisterClassHandler(typeof(TextBox),
            UIElement.GotKeyboardFocusEvent,
            new RoutedEventHandler(OnGotKeyboardFocus));

        EventManager.RegisterClassHandler(typeof(TextBox),
            UIElement.PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown));
    }

    private static void OnGotKeyboardFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb)
            tb.SelectAll();
    }

    private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBox tb) return;
        if (tb.IsKeyboardFocusWithin) return;

        tb.Focus();
        e.Handled = true;
    }
}

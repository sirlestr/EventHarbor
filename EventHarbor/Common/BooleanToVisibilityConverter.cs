using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EventHarbor.Common;

public class BooleanToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }
    public bool UseHidden { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool val = value is bool b && b;
        if (Invert) val = !val;
        return val ? Visibility.Visible : (UseHidden ? Visibility.Hidden : Visibility.Collapsed);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is Visibility v && v == Visibility.Visible ? !Invert : Invert;
}

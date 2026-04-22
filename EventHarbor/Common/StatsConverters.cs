using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EventHarbor.Common;

public class HexToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string hex && !string.IsNullOrWhiteSpace(hex))
        {
            try { return (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!; }
            catch { }
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}

public class PctToStarConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double v = 0;
        if (value is double d) v = d;
        else if (value is float f) v = f;

        v = Math.Max(0, Math.Min(1, v));
        if (Invert) v = 1 - v;
        return new GridLength(Math.Max(0.0001, v), GridUnitType.Star);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}

public class ZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int n = value switch
        {
            int i => i,
            long l => (int)l,
            double d => (int)d,
            _ => -1,
        };
        return n == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}

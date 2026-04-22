using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EventHarbor.Common;

public class StrengthTickConverter : IValueConverter
{
    private static readonly Brush Empty = new SolidColorBrush(Color.FromRgb(0xE8, 0xE2, 0xD5));
    private static readonly Brush Weak = new SolidColorBrush(Color.FromRgb(0xB8, 0x5C, 0x1F));
    private static readonly Brush Medium = new SolidColorBrush(Color.FromRgb(0xB8, 0x5C, 0x1F));
    private static readonly Brush Good = new SolidColorBrush(Color.FromRgb(0x8B, 0x7A, 0x12));
    private static readonly Brush Strong = new SolidColorBrush(Color.FromRgb(0x4A, 0x6B, 0x3D));

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int strength = value is int s ? s : 0;
        int index = parameter is string p && int.TryParse(p, out var i) ? i : 1;

        if (index > strength) return Empty;

        return strength switch
        {
            <= 1 => Weak,
            2 => Medium,
            3 => Good,
            _ => Strong,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}

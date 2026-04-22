using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace EventHarbor.Common;

public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Enum e) return string.Empty;
        return GetDescription(e);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;

    public static string GetDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field is null) return value.ToString();
        var attr = field.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? value.ToString();
    }
}

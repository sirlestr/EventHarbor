using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EventHarbor.Common.Controls;

public partial class IconView : UserControl
{
    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind), typeof(string), typeof(IconView),
        new PropertyMetadata(null, OnKindChanged));

    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
        nameof(Size), typeof(double), typeof(IconView),
        new PropertyMetadata(16.0, OnSizeChanged));

    public string? Kind
    {
        get => (string?)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public IconView()
    {
        InitializeComponent();
        Width = Size;
        Height = Size;
    }

    private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var icon = (IconView)d;
        if (e.NewValue is string kind && !string.IsNullOrEmpty(kind))
        {
            var key = $"Icon.{kind}";
            if (Application.Current?.TryFindResource(key) is Geometry geo)
                icon.PART_Path.Data = geo;
        }
    }

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var icon = (IconView)d;
        var size = (double)e.NewValue;
        icon.Width = size;
        icon.Height = size;
    }
}

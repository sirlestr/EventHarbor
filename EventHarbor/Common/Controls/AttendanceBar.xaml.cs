using System.Windows;
using System.Windows.Controls;
using EventHarbor.Domain;

namespace EventHarbor.Common.Controls;

public partial class AttendanceBar : UserControl
{
    public static readonly DependencyProperty EventProperty = DependencyProperty.Register(
        nameof(Event), typeof(CultureAction), typeof(AttendanceBar),
        new PropertyMetadata(null, OnEventChanged));

    public static readonly DependencyProperty BarHeightProperty = DependencyProperty.Register(
        nameof(BarHeight), typeof(double), typeof(AttendanceBar),
        new PropertyMetadata(6.0));

    public CultureAction? Event
    {
        get => (CultureAction?)GetValue(EventProperty);
        set => SetValue(EventProperty, value);
    }

    public double BarHeight
    {
        get => (double)GetValue(BarHeightProperty);
        set => SetValue(BarHeightProperty, value);
    }

    public AttendanceBar()
    {
        InitializeComponent();
        SizeChanged += (_, _) => Redraw();
    }

    private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((AttendanceBar)d).Redraw();

    private void Redraw()
    {
        var e = Event;
        var total = e is null ? 0 : e.TotalAttendance;
        var width = ActualWidth;

        if (total == 0 || width <= 0)
        {
            Seg0.Width = Seg1.Width = Seg2.Width = Seg3.Width = 0;
            EmptyLabel.Visibility = total == 0 ? Visibility.Visible : Visibility.Collapsed;
            return;
        }

        EmptyLabel.Visibility = Visibility.Collapsed;
        Seg0.Width = width * e!.Children / (double)total;
        Seg1.Width = width * e.Adults / (double)total;
        Seg2.Width = width * e.Seniors / (double)total;
        Seg3.Width = width * e.Disabled / (double)total;
    }
}

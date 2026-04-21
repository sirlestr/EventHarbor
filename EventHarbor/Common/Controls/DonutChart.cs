using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using EventHarbor.ViewModels;

namespace EventHarbor.Common.Controls;

public class DonutChart : Control
{
    static DonutChart()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(DonutChart),
            new FrameworkPropertyMetadata(typeof(DonutChart)));
    }

    public static readonly DependencyProperty SlicesProperty = DependencyProperty.Register(
        nameof(Slices), typeof(IEnumerable), typeof(DonutChart),
        new PropertyMetadata(null, OnSlicesChanged));

    public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
        nameof(InnerRadius), typeof(double), typeof(DonutChart),
        new PropertyMetadata(28.0));

    public static readonly DependencyProperty OuterRadiusProperty = DependencyProperty.Register(
        nameof(OuterRadius), typeof(double), typeof(DonutChart),
        new PropertyMetadata(46.0));

    public IEnumerable? Slices
    {
        get => (IEnumerable?)GetValue(SlicesProperty);
        set => SetValue(SlicesProperty, value);
    }

    public double InnerRadius
    {
        get => (double)GetValue(InnerRadiusProperty);
        set => SetValue(InnerRadiusProperty, value);
    }

    public double OuterRadius
    {
        get => (double)GetValue(OuterRadiusProperty);
        set => SetValue(OuterRadiusProperty, value);
    }

    private Canvas? _canvas;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _canvas = GetTemplateChild("PART_Canvas") as Canvas;
        Redraw();
    }

    private static void OnSlicesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var chart = (DonutChart)d;
        if (e.OldValue is INotifyCollectionChanged oldNcc)
            oldNcc.CollectionChanged -= chart.OnSliceCollectionChanged;
        if (e.NewValue is INotifyCollectionChanged newNcc)
            newNcc.CollectionChanged += chart.OnSliceCollectionChanged;
        chart.Redraw();
    }

    private void OnSliceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Redraw();

    private void Redraw()
    {
        if (_canvas is null) return;
        _canvas.Children.Clear();

        if (Slices is null) return;

        foreach (var item in Slices)
        {
            if (item is not OrganiserSlice s) continue;
            if (s.Visitors == 0) continue;

            var path = BuildArcPath(s.StartFraction, s.EndFraction, InnerRadius, OuterRadius);
            try
            {
                var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(s.ColorHex)!;
                path.Fill = brush;
            }
            catch
            {
                path.Fill = Brushes.Gray;
            }
            _canvas.Children.Add(path);
        }
    }

    private static Path BuildArcPath(double start, double end, double inner, double outer)
    {
        const double cx = 50, cy = 50;
        var startAngle = start * 2 * Math.PI - Math.PI / 2;
        var endAngle = end * 2 * Math.PI - Math.PI / 2;

        var p1 = new Point(cx + outer * Math.Cos(startAngle), cy + outer * Math.Sin(startAngle));
        var p2 = new Point(cx + outer * Math.Cos(endAngle), cy + outer * Math.Sin(endAngle));
        var p3 = new Point(cx + inner * Math.Cos(endAngle), cy + inner * Math.Sin(endAngle));
        var p4 = new Point(cx + inner * Math.Cos(startAngle), cy + inner * Math.Sin(startAngle));

        var largeArc = (end - start) > 0.5;

        var figure = new PathFigure { StartPoint = p1, IsClosed = true };
        figure.Segments.Add(new ArcSegment(p2, new Size(outer, outer), 0, largeArc, SweepDirection.Clockwise, true));
        figure.Segments.Add(new LineSegment(p3, true));
        figure.Segments.Add(new ArcSegment(p4, new Size(inner, inner), 0, largeArc, SweepDirection.Counterclockwise, true));

        var geo = new PathGeometry();
        geo.Figures.Add(figure);

        return new Path { Data = geo };
    }
}

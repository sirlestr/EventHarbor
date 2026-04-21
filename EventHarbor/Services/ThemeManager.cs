using System.Windows;

namespace EventHarbor.Services;

public class ThemeManager
{
    private readonly SettingsStore _settings;

    public ThemeManager(SettingsStore settings)
    {
        _settings = settings;
    }

    public AppTheme Current => _settings.Current.Theme;

    public void ApplyCurrent() => Apply(_settings.Current.Theme);

    public void Toggle()
    {
        var next = _settings.Current.Theme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
        _settings.SetTheme(next);
        Apply(next);
    }

    public void Apply(AppTheme theme)
    {
        var app = Application.Current;
        if (app is null) return;

        var resources = app.Resources.MergedDictionaries;

        // Remove any Colors.*.xaml already merged.
        for (int i = resources.Count - 1; i >= 0; i--)
        {
            var src = resources[i].Source?.OriginalString ?? string.Empty;
            if (src.Contains("Themes/Colors.", StringComparison.OrdinalIgnoreCase))
                resources.RemoveAt(i);
        }

        var target = theme == AppTheme.Dark
            ? "Themes/Colors.Dark.xaml"
            : "Themes/Colors.Light.xaml";

        var dict = new ResourceDictionary
        {
            Source = new Uri(target, UriKind.Relative),
        };

        // Keep palette dictionary before Typography/Controls so DynamicResource lookups resolve correctly.
        resources.Insert(0, dict);

        // Also flip WPF-UI theme if present.
        try
        {
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                theme == AppTheme.Dark
                    ? Wpf.Ui.Appearance.ApplicationTheme.Dark
                    : Wpf.Ui.Appearance.ApplicationTheme.Light);
        }
        catch
        {
            // WPF-UI may throw on startup ordering; ignore - our own palette is primary.
        }
    }
}

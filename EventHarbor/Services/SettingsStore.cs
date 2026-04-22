using System.IO;
using System.Text.Json;
using EventHarbor.Data;

namespace EventHarbor.Services;

public enum AppTheme { Light, Dark }

public class AppSettings
{
    public AppTheme Theme { get; set; } = AppTheme.Light;
}

public class SettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public AppSettings Current { get; private set; } = new();

    public SettingsStore() => Load();

    public void Load()
    {
        try
        {
            if (!File.Exists(AppPaths.SettingsPath)) return;
            var json = File.ReadAllText(AppPaths.SettingsPath);
            var s = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
            if (s is not null) Current = s;
        }
        catch
        {
            // corrupt file - keep defaults
        }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(AppPaths.LocalAppFolder);
            var json = JsonSerializer.Serialize(Current, JsonOptions);
            File.WriteAllText(AppPaths.SettingsPath, json);
        }
        catch
        {
            // best effort
        }
    }

    public void SetTheme(AppTheme theme)
    {
        Current.Theme = theme;
        Save();
    }
}

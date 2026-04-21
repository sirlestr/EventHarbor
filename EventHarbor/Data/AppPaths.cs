using System.IO;

namespace EventHarbor.Data;

public static class AppPaths
{
    public static string LocalAppFolder { get; } =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EventHarbor");

    public static string DatabasePath { get; } =
        Path.Combine(LocalAppFolder, "Data.db");

    public static string LogFolder { get; } =
        Path.Combine(LocalAppFolder, "logs");

    public static string SettingsPath { get; } =
        Path.Combine(LocalAppFolder, "settings.json");
}

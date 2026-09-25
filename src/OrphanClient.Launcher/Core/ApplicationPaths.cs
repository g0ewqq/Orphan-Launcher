using System.IO;

namespace OrphanClient.Launcher.Core;

public static class ApplicationPaths
{
    private const string LauncherFolder = "Launcher";

    public static string SettingsDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        ProductInfo.Product,
        LauncherFolder);

    public static string SettingsFile { get; } = Path.Combine(SettingsDirectory, "settings.json");

    public static string LogDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        ProductInfo.Product,
        LauncherFolder,
        "logs");

    public static string LogFile { get; } = Path.Combine(LogDirectory, "launcher.log");
}

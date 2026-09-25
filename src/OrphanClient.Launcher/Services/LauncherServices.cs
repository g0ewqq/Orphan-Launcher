using OrphanClient.Launcher.Accounts;
using OrphanClient.Launcher.Configuration;
using OrphanClient.Launcher.Core;
using OrphanClient.Launcher.Minecraft;

namespace OrphanClient.Launcher.Services;

public sealed class LauncherServices
{
    private readonly SettingsStore _store;

    public LauncherServices(SettingsStore store, LauncherSettings settings)
    {
        _store = store;
        Settings = settings;
        Locator = new MinecraftLocator();
        Launch = new LaunchCoordinator();
        Accounts = new AccountService();
    }

    public LauncherSettings Settings { get; }

    public MinecraftLocator Locator { get; }

    public LaunchCoordinator Launch { get; }

    public AccountService Accounts { get; }

    public void SyncStartupRegistration()
    {
        Settings.StartWithWindows = StartupRegistration.IsEnabled();
    }

    public bool ApplyStartWithWindows(bool enabled)
    {
        if (enabled && string.IsNullOrEmpty(Environment.ProcessPath))
        {
            Log.Warn("Could not register startup because the executable path is unavailable");
            return false;
        }

        if (!StartupRegistration.Apply(enabled, Environment.ProcessPath ?? string.Empty))
        {
            return false;
        }

        Settings.StartWithWindows = enabled;
        SaveSettings();
        return true;
    }

    public void ApplyMinecraftDirectory(string directory)
    {
        var normalized = directory.Trim();

        if (!string.Equals(Settings.MinecraftDirectory, normalized, StringComparison.OrdinalIgnoreCase))
        {
            Settings.MinecraftDirectory = normalized;
            SaveSettings();
        }

        RefreshLaunchState();
    }

    public void RefreshLaunchState() => Launch.Update(Locator.Locate(Settings.MinecraftDirectory));

    public void SaveSettings() => _store.Save(Settings);
}

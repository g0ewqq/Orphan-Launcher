using OrphanClient.Launcher.Configuration;
using OrphanClient.Launcher.Core;
using OrphanClient.Launcher.Services;
using OrphanClient.Launcher.UI;

namespace OrphanClient.Launcher.App;

public sealed class LauncherApplication
{
    private LauncherServices? _services;

    private MainWindow? _window;

    public void Start()
    {
        Log.Initialize(ApplicationPaths.LogFile, LogLevel.Info);
        Log.Info($"{ProductInfo.LauncherName} starting");

        var store = new SettingsStore();
        _services = new LauncherServices(store, store.Load());
        _services.SyncStartupRegistration();
        _services.RefreshLaunchState();

        _window = new MainWindow(_services);
        _window.Show();

        Log.Info("Launcher window shown");
    }

    public void Shutdown()
    {
        if (_services is null)
        {
            return;
        }

        _services.SaveSettings();
        Log.Info($"{ProductInfo.LauncherName} stopped");
        _services = null;
    }
}

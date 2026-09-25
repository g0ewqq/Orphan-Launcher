using System.Windows;
using OrphanClient.Launcher.Core;

namespace OrphanClient.Launcher.App;

public partial class App : Application
{
    private readonly LauncherApplication _launcher = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            _launcher.Start();
        }
        catch (Exception exception)
        {
            Log.Error("The launcher could not be started", exception);

            MessageBox.Show(
                $"{ProductInfo.LauncherName} could not be started.{Environment.NewLine}{Environment.NewLine}{exception.Message}",
                ProductInfo.LauncherName,
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _launcher.Shutdown();
        base.OnExit(e);
    }
}

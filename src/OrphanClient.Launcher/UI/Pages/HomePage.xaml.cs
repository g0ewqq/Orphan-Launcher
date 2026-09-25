using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OrphanClient.Launcher.Accounts;
using OrphanClient.Launcher.Core;
using OrphanClient.Launcher.Minecraft;
using OrphanClient.Launcher.Services;

namespace OrphanClient.Launcher.UI.Pages;

public partial class HomePage : UserControl
{
    private readonly LauncherServices _services;

    public HomePage(LauncherServices services)
    {
        _services = services;

        InitializeComponent();

        ClientValueText.Text = ProductInfo.ClientLabel;
        MinecraftValueText.Text = $"{ProductInfo.MinecraftVersion} {ProductInfo.MinecraftEdition}";

        ClientTileValueText.Text = ProductInfo.ClientVersion;
        ClientTileStateText.Text = "INSTALLED";
        MinecraftTileValueText.Text = ProductInfo.MinecraftVersion;
        AccountTileValueText.Text = AccountService.ProviderName.ToUpperInvariant();
        AccountTileStateText.Text = _services.Accounts.StatusText.ToUpperInvariant();
        AccountTileDot.Fill = StatusBrush(_services.Accounts.IsConnected);

        _services.Launch.StateChanged += OnLaunchStateChanged;

        ApplyLaunchState();
    }

    public event EventHandler? VersionsRequested;

    private void OnLaunchStateChanged(object? sender, EventArgs e) => ApplyLaunchState();

    private void ApplyLaunchState()
    {
        var launch = _services.Launch;

        switch (launch.State)
        {
            case LaunchState.Ready:
                LaunchButton.Content = "PLAY";
                LaunchButton.Tag = "PLAY NOW";
                LaunchButton.IsEnabled = true;
                break;

            case LaunchState.Unavailable:
                LaunchButton.Content = "UNAVAILABLE";
                LaunchButton.Tag = null;
                LaunchButton.IsEnabled = false;
                break;

            default:
                LaunchButton.Content = "CHECKING\u2026";
                LaunchButton.Tag = null;
                LaunchButton.IsEnabled = false;
                break;
        }

        var installation = launch.Installation;

        InstallationValueText.Text = DescribeInstallation(installation);
        InstallationStatusDot.Fill = StatusBrush(installation?.IsReady == true);
        MinecraftTileStateText.Text = DescribeMinecraft(installation);
        MinecraftTileDot.Fill = MinecraftTileBrush(installation);
        LaunchStatusText.Text = launch.Description;
    }

    private static string DescribeInstallation(MinecraftInstallation? installation) => installation?.Status switch
    {
        InstallationStatus.Installed => "Detected",
        InstallationStatus.NotInstalled => "Not found",
        InstallationStatus.NotConfigured => "Not configured",
        _ => "Checking"
    };

    private static string DescribeMinecraft(MinecraftInstallation? installation) => installation?.Status switch
    {
        InstallationStatus.Installed => "DETECTED",
        InstallationStatus.NotInstalled => "NOT FOUND",
        _ => "NOT SET"
    };

    private Brush StatusBrush(bool positive) =>
        (Brush)FindResource(positive ? "PrimaryBrush" : "TextMutedBrush");

    private Brush MinecraftTileBrush(MinecraftInstallation? installation) => (Brush)FindResource(installation?.Status switch
    {
        InstallationStatus.Installed => "PrimaryBrush",
        InstallationStatus.NotInstalled => "WarningBrush",
        _ => "TextMutedBrush"
    });

    private void OnViewDetailsClick(object sender, RoutedEventArgs e) => VersionsRequested?.Invoke(this, EventArgs.Empty);

    private void OnLaunchClick(object sender, RoutedEventArgs e) =>
        LaunchStatusText.Text = _services.Launch.RequestLaunch();
}

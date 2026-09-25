using OrphanClient.Launcher.Minecraft;

namespace OrphanClient.Launcher.Core;

public sealed class LaunchCoordinator
{
    public LaunchState State { get; private set; } = LaunchState.Checking;

    public MinecraftInstallation? Installation { get; private set; }

    public event EventHandler? StateChanged;

    public string Description => State switch
    {
        LaunchState.Ready => $"{ProductInfo.MinecraftLabel} detected.",
        LaunchState.Unavailable when Installation?.Status == InstallationStatus.NotConfigured => "Set the Minecraft directory in Settings to locate the game.",
        LaunchState.Unavailable => $"Minecraft {ProductInfo.MinecraftVersion} was not found in {Installation?.Directory}.",
        _ => "Checking for Minecraft\u2026"
    };

    public void Update(MinecraftInstallation installation)
    {
        Installation = installation;
        State = installation.IsReady ? LaunchState.Ready : LaunchState.Unavailable;

        var location = string.IsNullOrWhiteSpace(installation.Directory) ? "an unconfigured directory" : installation.Directory;
        Log.Info($"Minecraft status {installation.Status} for {location}");

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public string RequestLaunch()
    {
        switch (State)
        {
            case LaunchState.Ready:
                Log.Info("Launch requested but the launch pipeline is not implemented");
                return "The launch pipeline is not implemented in this phase.";

            case LaunchState.Unavailable:
                Log.Warn($"Launch requested while Minecraft is unavailable: {Description}");
                return Description;

            default:
                return "The launcher is still checking for Minecraft.";
        }
    }
}

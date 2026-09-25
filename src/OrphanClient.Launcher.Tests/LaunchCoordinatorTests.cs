using OrphanClient.Launcher.Core;
using OrphanClient.Launcher.Minecraft;
using Xunit;

namespace OrphanClient.Launcher.Tests;

public sealed class LaunchCoordinatorTests
{
    [Fact]
    public void Starts_in_the_checking_state()
    {
        var coordinator = new LaunchCoordinator();

        Assert.Equal(LaunchState.Checking, coordinator.State);
        Assert.Null(coordinator.Installation);
        Assert.Equal("Checking for Minecraft\u2026", coordinator.Description);
        Assert.Equal("The launcher is still checking for Minecraft.", coordinator.RequestLaunch());
    }

    [Fact]
    public void Becomes_ready_when_the_installation_is_present()
    {
        var coordinator = new LaunchCoordinator();
        var notifications = 0;
        coordinator.StateChanged += (_, _) => notifications++;

        coordinator.Update(new MinecraftInstallation(@"C:\game", InstallationStatus.Installed));

        Assert.Equal(LaunchState.Ready, coordinator.State);
        Assert.Equal(1, notifications);
        Assert.Equal("Minecraft 1.8.9 detected.", coordinator.Description);
        Assert.Equal("The launch pipeline is not implemented in this phase.", coordinator.RequestLaunch());
    }

    [Fact]
    public void Becomes_unavailable_when_no_directory_is_configured()
    {
        var coordinator = new LaunchCoordinator();

        coordinator.Update(new MinecraftInstallation(string.Empty, InstallationStatus.NotConfigured));

        Assert.Equal(LaunchState.Unavailable, coordinator.State);
        Assert.Equal("Set the Minecraft directory in Settings to locate the game.", coordinator.Description);
        Assert.Equal(coordinator.Description, coordinator.RequestLaunch());
    }

    [Fact]
    public void Becomes_unavailable_when_the_version_folder_is_missing()
    {
        var coordinator = new LaunchCoordinator();

        coordinator.Update(new MinecraftInstallation(@"C:\game", InstallationStatus.NotInstalled));

        Assert.Equal(LaunchState.Unavailable, coordinator.State);
        Assert.Equal(@"Minecraft 1.8.9 was not found in C:\game.", coordinator.Description);
        Assert.Equal(coordinator.Description, coordinator.RequestLaunch());
    }
}

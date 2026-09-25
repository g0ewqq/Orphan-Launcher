using System.IO;
using OrphanClient.Launcher.Core;
using OrphanClient.Launcher.Minecraft;
using Xunit;

namespace OrphanClient.Launcher.Tests;

public sealed class MinecraftLocatorTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        "orphan-launcher-tests",
        Guid.NewGuid().ToString("N"));

    private readonly MinecraftLocator _locator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Reports_an_unconfigured_directory(string? directory)
    {
        var installation = _locator.Locate(directory);

        Assert.Equal(InstallationStatus.NotConfigured, installation.Status);
        Assert.False(installation.IsReady);
        Assert.Equal(string.Empty, installation.Directory);
    }

    [Fact]
    public void Reports_a_missing_version_folder()
    {
        Directory.CreateDirectory(_directory);

        var installation = _locator.Locate(_directory);

        Assert.Equal(InstallationStatus.NotInstalled, installation.Status);
        Assert.False(installation.IsReady);
        Assert.Equal(_directory, installation.Directory);
    }

    [Fact]
    public void Reports_an_installed_version_folder()
    {
        Directory.CreateDirectory(Path.Combine(_directory, "versions", ProductInfo.MinecraftVersion));

        var installation = _locator.Locate(_directory);

        Assert.Equal(InstallationStatus.Installed, installation.Status);
        Assert.True(installation.IsReady);
    }

    [Fact]
    public void Trims_surrounding_whitespace()
    {
        Directory.CreateDirectory(Path.Combine(_directory, "versions", ProductInfo.MinecraftVersion));

        var installation = _locator.Locate($"  {_directory}  ");

        Assert.Equal(_directory, installation.Directory);
        Assert.Equal(InstallationStatus.Installed, installation.Status);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}

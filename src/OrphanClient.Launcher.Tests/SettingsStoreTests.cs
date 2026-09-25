using System.IO;
using OrphanClient.Launcher.Configuration;
using Xunit;

namespace OrphanClient.Launcher.Tests;

public sealed class SettingsStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        "orphan-launcher-tests",
        Guid.NewGuid().ToString("N"));

    private string SettingsFile => Path.Combine(_directory, "settings.json");

    [Fact]
    public void Returns_defaults_when_the_file_is_missing()
    {
        var settings = new SettingsStore(SettingsFile).Load();

        Assert.Equal(string.Empty, settings.MinecraftDirectory);
        Assert.True(settings.AnimationsEnabled);
        Assert.False(settings.StartWithWindows);
        Assert.False(settings.WindowMaximized);
    }

    [Fact]
    public void Round_trips_settings_through_disk()
    {
        var store = new SettingsStore(SettingsFile);
        var settings = store.Load();

        settings.MinecraftDirectory = @"C:\games\.minecraft";
        settings.AnimationsEnabled = false;
        settings.StartWithWindows = true;
        settings.WindowWidth = 1280;
        settings.WindowHeight = 800;
        settings.WindowMaximized = true;

        Assert.True(store.Save(settings));

        var reloaded = store.Load();

        Assert.Equal(@"C:\games\.minecraft", reloaded.MinecraftDirectory);
        Assert.False(reloaded.AnimationsEnabled);
        Assert.True(reloaded.StartWithWindows);
        Assert.Equal(1280, reloaded.WindowWidth);
        Assert.Equal(800, reloaded.WindowHeight);
        Assert.True(reloaded.WindowMaximized);
    }

    [Fact]
    public void Returns_defaults_when_the_file_is_malformed()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(SettingsFile, "{ not valid json");

        var settings = new SettingsStore(SettingsFile).Load();

        Assert.Equal(string.Empty, settings.MinecraftDirectory);
        Assert.True(settings.AnimationsEnabled);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}

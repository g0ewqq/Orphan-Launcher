using System.IO;
using OrphanClient.Launcher.Core;

namespace OrphanClient.Launcher.Minecraft;

public sealed class MinecraftLocator
{
    public MinecraftInstallation Locate(string? directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
        {
            return new MinecraftInstallation(string.Empty, InstallationStatus.NotConfigured);
        }

        var root = directory.Trim();
        var versionDirectory = Path.Combine(root, "versions", ProductInfo.MinecraftVersion);
        var status = Directory.Exists(versionDirectory) ? InstallationStatus.Installed : InstallationStatus.NotInstalled;

        return new MinecraftInstallation(root, status);
    }
}

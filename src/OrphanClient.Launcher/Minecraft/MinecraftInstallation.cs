namespace OrphanClient.Launcher.Minecraft;

public sealed record MinecraftInstallation(string Directory, InstallationStatus Status)
{
    public bool IsReady => Status == InstallationStatus.Installed;
}

namespace OrphanClient.Launcher.Core;

public sealed record ClientVersion(string Version, string MinecraftVersion, bool IsCurrent)
{
    public string ClientLabel => $"{ProductInfo.Product} {Version}";

    public string MinecraftLabel => $"Minecraft {MinecraftVersion}";
}

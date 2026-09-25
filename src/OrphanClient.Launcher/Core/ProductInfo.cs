namespace OrphanClient.Launcher.Core;

public static class ProductInfo
{
    public const string Product = "OrphanClient";
    public const string LauncherName = "OrphanClient Launcher";
    public const string ClientVersion = "V1";
    public const string MinecraftVersion = "1.8.9";
    public const string MinecraftEdition = "Java Edition";

    public static string ClientLabel => $"{Product} {ClientVersion}";

    public static string MinecraftLabel => $"Minecraft {MinecraftVersion}";
}

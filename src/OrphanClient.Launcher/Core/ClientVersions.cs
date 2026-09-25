namespace OrphanClient.Launcher.Core;

public static class ClientVersions
{
    public static IReadOnlyList<ClientVersion> Available { get; } =
    [
        new ClientVersion(ProductInfo.ClientVersion, ProductInfo.MinecraftVersion, IsCurrent: true)
    ];
}

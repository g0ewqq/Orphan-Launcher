using OrphanClient.Launcher.Core;
using Xunit;

namespace OrphanClient.Launcher.Tests;

public sealed class ProductInfoTests
{
    [Fact]
    public void Exposes_the_configured_identity()
    {
        Assert.Equal("OrphanClient", ProductInfo.Product);
        Assert.Equal("OrphanClient Launcher", ProductInfo.LauncherName);
        Assert.Equal("V1", ProductInfo.ClientVersion);
        Assert.Equal("1.8.9", ProductInfo.MinecraftVersion);
        Assert.Equal("Java Edition", ProductInfo.MinecraftEdition);
    }

    [Fact]
    public void Builds_the_display_labels()
    {
        Assert.Equal("OrphanClient V1", ProductInfo.ClientLabel);
        Assert.Equal("Minecraft 1.8.9", ProductInfo.MinecraftLabel);
    }
}

using OrphanClient.Launcher.Accounts;
using Xunit;

namespace OrphanClient.Launcher.Tests;

public sealed class AccountServiceTests
{
    [Fact]
    public void Starts_disconnected()
    {
        var accounts = new AccountService();

        Assert.False(accounts.IsConnected);
        Assert.Equal("Discord", AccountService.ProviderName);
        Assert.Equal("Not Connected", accounts.StatusText);
        Assert.Equal("Connect Discord", accounts.ConnectActionText);
    }

    [Fact]
    public void Reports_that_signing_in_is_not_implemented()
    {
        var accounts = new AccountService();

        Assert.Equal("Discord sign-in is not implemented in this phase.", accounts.RequestConnect());
        Assert.False(accounts.IsConnected);
    }
}

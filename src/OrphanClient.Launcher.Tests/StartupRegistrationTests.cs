using OrphanClient.Launcher.Services;
using Xunit;

namespace OrphanClient.Launcher.Tests;

public sealed class StartupRegistrationTests
{
    private const string ExecutablePath = @"C:\apps\OrphanClient.Launcher.exe";

    [Fact]
    public void Enables_and_disables_the_per_user_startup_entry()
    {
        Assert.True(StartupRegistration.Apply(false, ExecutablePath));
        Assert.False(StartupRegistration.IsEnabled());

        try
        {
            Assert.True(StartupRegistration.Apply(true, ExecutablePath));
            Assert.True(StartupRegistration.IsEnabled());
        }
        finally
        {
            StartupRegistration.Apply(false, ExecutablePath);
        }

        Assert.False(StartupRegistration.IsEnabled());
    }
}

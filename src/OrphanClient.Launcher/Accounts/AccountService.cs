using OrphanClient.Launcher.Core;

namespace OrphanClient.Launcher.Accounts;

public sealed class AccountService
{
    public const string ProviderName = "Discord";

    public bool IsConnected { get; private set; }

    public string StatusText => IsConnected ? "Connected" : "Not Connected";

    public string ConnectActionText => $"Connect {ProviderName}";

    public string RequestConnect()
    {
        Log.Info("Discord connection requested but the OAuth2 flow is not implemented");
        return "Discord sign-in is not implemented in this phase.";
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OrphanClient.Launcher.Accounts;
using OrphanClient.Launcher.Services;

namespace OrphanClient.Launcher.UI.Pages;

public partial class AccountPage : UserControl
{
    private readonly AccountService _accounts;

    public AccountPage(LauncherServices services)
    {
        _accounts = services.Accounts;

        InitializeComponent();

        ProviderText.Text = AccountService.ProviderName;
        AccountStatusText.Text = _accounts.StatusText;
        AccountStatusDot.Fill = (Brush)FindResource(_accounts.IsConnected ? "PrimaryBrush" : "TextMutedBrush");
        ConnectButton.Content = _accounts.ConnectActionText;
    }

    private void OnConnectClick(object sender, RoutedEventArgs e) =>
        ConnectResultText.Text = _accounts.RequestConnect();
}

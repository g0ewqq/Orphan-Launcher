using System.Windows.Controls;
using OrphanClient.Launcher.Core;

namespace OrphanClient.Launcher.UI.Pages;

public partial class VersionsPage : UserControl
{
    public VersionsPage()
    {
        InitializeComponent();

        VersionList.ItemsSource = ClientVersions.Available;

        var count = ClientVersions.Available.Count;

        VersionSummaryText.Text = count == 1
            ? "1 client version is available in this build."
            : $"{count} client versions are available in this build.";
    }
}

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using OrphanClient.Launcher.Core;
using OrphanClient.Launcher.Minecraft;
using OrphanClient.Launcher.Services;

namespace OrphanClient.Launcher.UI.Pages;

public partial class SettingsPage : UserControl
{
    private readonly LauncherServices _services;

    private bool _applyingStartup;

    public SettingsPage(LauncherServices services)
    {
        _services = services;

        InitializeComponent();

        MinecraftDirectoryBox.Text = _services.Settings.MinecraftDirectory;
        StartWithWindowsToggle.IsChecked = _services.Settings.StartWithWindows;
        AnimationsToggle.IsChecked = _services.Settings.AnimationsEnabled;

        SettingsPathText.Text = ApplicationPaths.SettingsFile;
        LogPathText.Text = ApplicationPaths.LogFile;

        StartWithWindowsToggle.Checked += OnStartWithWindowsChanged;
        StartWithWindowsToggle.Unchecked += OnStartWithWindowsChanged;
        AnimationsToggle.Checked += OnAnimationsChanged;
        AnimationsToggle.Unchecked += OnAnimationsChanged;

        UpdateMinecraftStatus();
    }

    private void OnStartWithWindowsChanged(object sender, RoutedEventArgs e)
    {
        if (_applyingStartup)
        {
            return;
        }

        _applyingStartup = true;

        try
        {
            var applied = _services.ApplyStartWithWindows(StartWithWindowsToggle.IsChecked == true);

            StartWithWindowsToggle.IsChecked = _services.Settings.StartWithWindows;
            StartWithWindowsStatusText.Text = applied ? string.Empty : "The startup entry could not be updated.";
        }
        finally
        {
            _applyingStartup = false;
        }
    }

    private void OnAnimationsChanged(object sender, RoutedEventArgs e)
    {
        _services.Settings.AnimationsEnabled = AnimationsToggle.IsChecked == true;
        _services.SaveSettings();
    }

    private void OnMinecraftDirectoryLostFocus(object sender, RoutedEventArgs e) => CommitMinecraftDirectory();

    private void OnMinecraftDirectoryKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        CommitMinecraftDirectory();
        e.Handled = true;
    }

    private void OnBrowseClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog { Title = "Select the Minecraft directory" };
        var current = MinecraftDirectoryBox.Text.Trim();

        if (Directory.Exists(current))
        {
            dialog.InitialDirectory = current;
        }

        if (dialog.ShowDialog() == true)
        {
            MinecraftDirectoryBox.Text = dialog.FolderName;
            CommitMinecraftDirectory();
        }
    }

    private void CommitMinecraftDirectory()
    {
        _services.ApplyMinecraftDirectory(MinecraftDirectoryBox.Text);
        MinecraftDirectoryBox.Text = _services.Settings.MinecraftDirectory;

        UpdateMinecraftStatus();
    }

    private void UpdateMinecraftStatus()
    {
        var installation = _services.Locator.Locate(_services.Settings.MinecraftDirectory);

        MinecraftStatusText.Text = installation.Status switch
        {
            InstallationStatus.Installed => $"{ProductInfo.MinecraftLabel} found in this directory.",
            InstallationStatus.NotInstalled => $"{ProductInfo.MinecraftLabel} was not found in this directory.",
            _ => "No Minecraft directory is configured."
        };
    }
}

using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using OrphanClient.Launcher.Core;
using OrphanClient.Launcher.Services;
using OrphanClient.Launcher.UI.Pages;

namespace OrphanClient.Launcher.UI;

public partial class MainWindow : Window
{
    private static readonly TimeSpan NavigationDuration = TimeSpan.FromMilliseconds(170);

    private readonly LauncherServices _services;
    private readonly HomePage _homePage;
    private readonly VersionsPage _versionsPage;
    private readonly SettingsPage _settingsPage;
    private readonly AccountPage _accountPage;

    private UserControl? _currentPage;

    public MainWindow(LauncherServices services)
    {
        _services = services;

        InitializeComponent();

        Title = ProductInfo.LauncherName;

        _homePage = new HomePage(services);
        _versionsPage = new VersionsPage();
        _settingsPage = new SettingsPage(services);
        _accountPage = new AccountPage(services);

        _homePage.VersionsRequested += OnVersionsRequested;

        RestoreWindowPlacement();

        StateChanged += OnWindowStateChanged;
        Closing += OnWindowClosing;
        _services.Launch.StateChanged += OnLaunchStateChanged;

        UpdateStatusStrip();
        HomeNavItem.IsChecked = true;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        WorkAreaMaximizer.Attach(this);
    }

    private void RestoreWindowPlacement()
    {
        var settings = _services.Settings;

        Width = Math.Max(MinWidth, settings.WindowWidth);
        Height = Math.Max(MinHeight, settings.WindowHeight);

        if (settings.WindowMaximized)
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void Navigate(UserControl page)
    {
        if (ReferenceEquals(_currentPage, page))
        {
            return;
        }

        _currentPage = page;
        PageHost.Content = page;

        page.BeginAnimation(OpacityProperty, null);
        page.Opacity = 1;

        if (page.RenderTransform is not TranslateTransform slide)
        {
            slide = new TranslateTransform();
            page.RenderTransform = slide;
        }

        slide.BeginAnimation(TranslateTransform.XProperty, null);
        slide.X = 0;

        if (!_services.Settings.AnimationsEnabled)
        {
            return;
        }

        var easing = new CubicEase { EasingMode = EasingMode.EaseOut };

        page.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, NavigationDuration) { EasingFunction = easing });
        slide.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(16, 0, NavigationDuration) { EasingFunction = easing });
    }

    private void OnNavigationChecked(object sender, RoutedEventArgs e)
    {
        if (sender == HomeNavItem)
        {
            Navigate(_homePage);
        }
        else if (sender == VersionsNavItem)
        {
            Navigate(_versionsPage);
        }
        else if (sender == SettingsNavItem)
        {
            Navigate(_settingsPage);
        }
        else if (sender == AccountNavItem)
        {
            Navigate(_accountPage);
        }
    }

    private void OnVersionsRequested(object? sender, EventArgs e) => VersionsNavItem.IsChecked = true;

    private void OnLaunchStateChanged(object? sender, EventArgs e) => UpdateStatusStrip();

    private void UpdateStatusStrip()
    {
        var launch = _services.Launch;

        StatusStripText.Text = launch.Description;
        StatusIndicator.Fill = (Brush)FindResource(launch.State switch
        {
            LaunchState.Ready => "PrimaryBrush",
            LaunchState.Unavailable => "WarningBrush",
            _ => "TextMutedBrush"
        });
    }

    private void OnWindowStateChanged(object? sender, EventArgs e)
    {
        var maximized = WindowState == WindowState.Maximized;

        MaximizeButton.Content = maximized ? "\uE923" : "\uE922";
        AutomationProperties.SetName(MaximizeButton, maximized ? "Restore" : "Maximize");
    }

    private void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        var settings = _services.Settings;
        settings.WindowMaximized = WindowState == WindowState.Maximized;

        if (WindowState == WindowState.Normal)
        {
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
        }
    }

    private void OnMinimizeClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void OnMaximizeClick(object sender, RoutedEventArgs e) =>
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
}

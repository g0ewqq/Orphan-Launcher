namespace OrphanClient.Launcher.Configuration;

public sealed class LauncherSettings
{
    public string MinecraftDirectory { get; set; } = string.Empty;

    public bool AnimationsEnabled { get; set; } = true;

    public bool StartWithWindows { get; set; }

    public double WindowWidth { get; set; } = 1120;

    public double WindowHeight { get; set; } = 700;

    public bool WindowMaximized { get; set; }
}

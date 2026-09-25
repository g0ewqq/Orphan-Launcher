using System.IO;
using Microsoft.Win32;
using OrphanClient.Launcher.Core;

namespace OrphanClient.Launcher.Services;

public static class StartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "OrphanClient Launcher";

    public static bool IsEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            return key?.GetValue(ValueName) is string value && value.Length > 0;
        }
        catch (Exception exception) when (IsAccessFailure(exception))
        {
            Log.Warn($"Could not read the startup entry: {exception.Message}");
            return false;
        }
    }

    public static bool Apply(bool enabled, string executablePath)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);

            if (key is null)
            {
                Log.Warn("The per-user startup registry key is not available");
                return false;
            }

            if (enabled)
            {
                key.SetValue(ValueName, $"\"{executablePath}\"");
            }
            else
            {
                key.DeleteValue(ValueName, throwOnMissingValue: false);
            }

            Log.Info($"Start with Windows set to {enabled}");
            return true;
        }
        catch (Exception exception) when (IsAccessFailure(exception))
        {
            Log.Error("Could not update the startup entry", exception);
            return false;
        }
    }

    private static bool IsAccessFailure(Exception exception) =>
        exception is UnauthorizedAccessException or IOException or System.Security.SecurityException;
}

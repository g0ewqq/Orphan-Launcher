using System.IO;
using System.Text.Json;
using OrphanClient.Launcher.Core;

namespace OrphanClient.Launcher.Configuration;

public sealed class SettingsStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public SettingsStore(string? filePath = null)
    {
        FilePath = filePath ?? ApplicationPaths.SettingsFile;
    }

    public string FilePath { get; }

    public LauncherSettings Load()
    {
        if (!File.Exists(FilePath))
        {
            return new LauncherSettings();
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<LauncherSettings>(json, SerializerOptions) ?? new LauncherSettings();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
        {
            Log.Warn($"Could not read settings from {FilePath}: {exception.Message}");
            return new LauncherSettings();
        }
    }

    public bool Save(LauncherSettings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(FilePath);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, SerializerOptions));
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Log.Error($"Could not write settings to {FilePath}", exception);
            return false;
        }
    }
}

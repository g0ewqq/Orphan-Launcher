using System.Globalization;
using System.IO;

namespace OrphanClient.Launcher.Core;

public static class Log
{
    private static readonly object Sync = new();

    private static string? _filePath;

    public static LogLevel MinimumLevel { get; set; } = LogLevel.Info;

    public static void Initialize(string filePath, LogLevel minimumLevel)
    {
        lock (Sync)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _filePath = filePath;
            MinimumLevel = minimumLevel;
        }
    }

    public static void Debug(string message) => Write(LogLevel.Debug, message);

    public static void Info(string message) => Write(LogLevel.Info, message);

    public static void Warn(string message) => Write(LogLevel.Warn, message);

    public static void Error(string message) => Write(LogLevel.Error, message);

    public static void Error(string message, Exception exception) => Write(LogLevel.Error, $"{message}: {exception}");

    private static void Write(LogLevel level, string message)
    {
        if (level < MinimumLevel)
        {
            return;
        }

        var line = string.Format(
            CultureInfo.InvariantCulture,
            "{0:yyyy-MM-dd HH:mm:ss} [{1,-5}] {2}",
            DateTime.Now,
            level.ToString().ToUpperInvariant(),
            message);

        lock (Sync)
        {
            System.Diagnostics.Debug.WriteLine(line);

            if (_filePath is null)
            {
                return;
            }

            try
            {
                File.AppendAllText(_filePath, line + Environment.NewLine);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}

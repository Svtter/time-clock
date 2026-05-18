using System;
using System.IO;

namespace TimeClock.Helpers;

public static class Logger
{
    private static readonly string LogDirectory = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TimeClock", "Logs");

    private static readonly object Lock = new();

    public static void Info(string message) => Write("INFO", message);
    public static void Warn(string message) => Write("WARN", message);
    public static void Error(string message) => Write("ERROR", message);

    private static void Write(string level, string message)
    {
        try
        {
            lock (Lock)
            {
                Directory.CreateDirectory(LogDirectory);
                var logFile = System.IO.Path.Combine(LogDirectory, $"clock_{DateTime.Now:yyyyMMdd}.log");
                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
                File.AppendAllText(logFile, line);
            }
        }
        catch
        {
        }
    }

    public static string GetTodayLogFile()
    {
        return System.IO.Path.Combine(LogDirectory, $"clock_{DateTime.Now:yyyyMMdd}.log");
    }

    public static string GetLogDirectory()
    {
        return LogDirectory;
    }
}

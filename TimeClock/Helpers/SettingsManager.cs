using System;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace TimeClock.Helpers;

public class AppSettings
{
    public bool AutoStart { get; set; }
    public bool ShowSecondHand { get; set; } = true;
    public double WindowLeft { get; set; } = double.NaN;
    public double WindowTop { get; set; } = double.NaN;
}

public static class SettingsManager
{
    private static readonly string SettingsDir = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "TimeClock");

    private static readonly string SettingsFilePath = System.IO.Path.Combine(
        SettingsDir, "settings.json");

    public static AppSettings Current { get; private set; } = new();

    public static event Action<AppSettings>? SettingsChanged;

    public static void Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                Current = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                Logger.Info("Settings loaded");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to load settings: {ex.Message}");
        }
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(SettingsDir);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
            Current = settings;
            SetAutoStart(settings.AutoStart);
            Logger.Info($"Settings saved: AutoStart={settings.AutoStart}, ShowSecondHand={settings.ShowSecondHand}");
            SettingsChanged?.Invoke(settings);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to save settings: {ex.Message}");
        }
    }

    public static void SaveWindowPosition(double left, double top)
    {
        try
        {
            Current.WindowLeft = left;
            Current.WindowTop = top;
            Directory.CreateDirectory(SettingsDir);
            var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch
        {
        }
    }

    public static void SetAutoStart(bool enable)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (enable)
            {
                var exePath = Environment.ProcessPath ?? "";
                key?.SetValue("TimeClock", $"\"{exePath}\"");
                Logger.Info("Auto-start enabled");
            }
            else
            {
                key?.DeleteValue("TimeClock", false);
                Logger.Info("Auto-start disabled");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to set auto-start: {ex.Message}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using TimeClock.Helpers;

namespace TimeClock;

public class AlarmEditItem
{
    public bool Enabled { get; set; } = true;
    public string Time { get; set; } = "07:30";
    public string Label { get; set; } = "";
}

public partial class SettingsWindow : Window
{
    private readonly ObservableCollection<AlarmEditItem> _alarms = new();

    public SettingsWindow()
    {
        InitializeComponent();
        var settings = SettingsManager.Current;
        ChkAutoStart.IsChecked = settings.AutoStart;
        ChkSecondHand.IsChecked = settings.ShowSecondHand;
        foreach (var alarm in settings.Alarms)
        {
            _alarms.Add(new AlarmEditItem
            {
                Enabled = alarm.Enabled,
                Time = alarm.Time,
                Label = alarm.Label,
            });
        }
        LstAlarms.ItemsSource = _alarms;
    }

    private void BtnAddAlarm_Click(object sender, RoutedEventArgs e)
    {
        _alarms.Add(new AlarmEditItem());
        LstAlarms.SelectedIndex = _alarms.Count - 1;
        LstAlarms.ScrollIntoView(LstAlarms.SelectedItem);
    }

    private void BtnRemoveAlarm_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button { DataContext: AlarmEditItem item })
        {
            _alarms.Remove(item);
        }
    }

    private void BtnOK_Click(object sender, RoutedEventArgs e)
    {
        var alarms = new List<AlarmSettings>();
        foreach (var item in _alarms)
        {
            var time = NormalizeAlarmTime(item.Time);
            if (time is null)
            {
                System.Windows.MessageBox.Show(
                    $"闹钟时间 \"{item.Time}\" 格式无效，请使用 HH:mm 格式（例如 07:30）。",
                    "设置", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            alarms.Add(new AlarmSettings
            {
                Enabled = item.Enabled,
                Time = time,
                Label = item.Label?.Trim() ?? "",
            });
        }

        var settings = new AppSettings
        {
            AutoStart = ChkAutoStart.IsChecked ?? false,
            ShowSecondHand = ChkSecondHand.IsChecked ?? true,
            WindowLeft = SettingsManager.Current.WindowLeft,
            WindowTop = SettingsManager.Current.WindowTop,
            Alarms = alarms,
        };
        SettingsManager.Save(settings);
        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void BtnOpenLog_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dir = Logger.GetLogDirectory();
            Directory.CreateDirectory(dir);
            System.Diagnostics.Process.Start("explorer.exe", dir);
            Logger.Info("Opened log directory");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to open log directory: {ex.Message}");
            System.Windows.MessageBox.Show($"无法打开日志文件夹: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static string? NormalizeAlarmTime(string? text)
    {
        if (TimeSpan.TryParseExact(text?.Trim(), @"hh\:mm", null, out var time) ||
            TimeSpan.TryParseExact(text?.Trim(), @"h\:mm", null, out time))
        {
            return $"{time.Hours:00}:{time.Minutes:00}";
        }
        return null;
    }
}

using System;
using System.IO;
using System.Windows;
using TimeClock.Helpers;

namespace TimeClock;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        var settings = SettingsManager.Current;
        ChkAutoStart.IsChecked = settings.AutoStart;
        ChkSecondHand.IsChecked = settings.ShowSecondHand;
    }

    private void BtnOK_Click(object sender, RoutedEventArgs e)
    {
        var settings = new AppSettings
        {
            AutoStart = ChkAutoStart.IsChecked ?? false,
            ShowSecondHand = ChkSecondHand.IsChecked ?? true,
            WindowLeft = SettingsManager.Current.WindowLeft,
            WindowTop = SettingsManager.Current.WindowTop,
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
}

using System;
using System.IO;
using System.Windows;
using TimeClock.Helpers;

namespace TimeClock;

public partial class LogWindow : Window
{
    public LogWindow()
    {
        InitializeComponent();
        LoadLog();
    }

    private void LoadLog()
    {
        try
        {
            var logFile = Logger.GetTodayLogFile();
            if (File.Exists(logFile))
            {
                LogContent.Text = File.ReadAllText(logFile);
                LogContent.ScrollToEnd();
            }
            else
            {
                LogContent.Text = "暂无日志记录。";
            }
        }
        catch (Exception ex)
        {
            LogContent.Text = $"读取日志失败: {ex.Message}";
        }
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        LoadLog();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}

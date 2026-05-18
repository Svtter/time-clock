using System;
using System.Drawing;
using System.IO;
using System.Windows;
using TimeClock.Helpers;

namespace TimeClock;

public partial class App : System.Windows.Application
{
    private System.Windows.Forms.NotifyIcon? _notifyIcon;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        SettingsManager.Load();
        Logger.Info("Application started");

        CreateTrayIcon();

        var mainWindow = new MainWindow();
        mainWindow.RestorePosition();
        mainWindow.Show();
        Current.MainWindow = mainWindow;
    }

    private void CreateTrayIcon()
    {
        _notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            Icon = CreateClockIcon(),
            Text = "TimeClock",
            Visible = true,
        };

        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add("设置...", null, (_, _) => OpenSettings());
        menu.Items.Add("查看日志", null, (_, _) => OpenLog());
        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
        menu.Items.Add("退出", null, (_, _) => Shutdown());

        _notifyIcon.ContextMenuStrip = menu;
        _notifyIcon.DoubleClick += (_, _) => Current.MainWindow?.Show();
    }

    private void OpenSettings()
    {
        if (Current.MainWindow is MainWindow mw)
        {
            var win = new SettingsWindow { Owner = mw };
            win.ShowDialog();
        }
    }

    private void OpenLog()
    {
        if (Current.MainWindow is MainWindow mw)
        {
            var win = new LogWindow { Owner = mw };
            win.ShowDialog();
        }
    }

    private static Icon CreateClockIcon()
    {
        using var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        using var circlePen = new System.Drawing.Pen(System.Drawing.Color.White, 2);
        g.DrawEllipse(circlePen, 3, 3, 26, 26);

        using var hourPen = new System.Drawing.Pen(System.Drawing.Color.White, 3);
        g.DrawLine(hourPen, 16, 16, 16, 6);

        using var minutePen = new System.Drawing.Pen(System.Drawing.Color.White, 2);
        g.DrawLine(minutePen, 16, 16, 26, 16);

        g.FillEllipse(System.Drawing.Brushes.Orange, 14, 14, 4, 4);

        var hIcon = bmp.GetHicon();
        return Icon.FromHandle(hIcon);
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        Logger.Info("Application exited");
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
    }
}

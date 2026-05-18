using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TimeClock.Helpers;

namespace TimeClock;

public partial class MainWindow : Window
{
    private const double Center = 129;
    private const double ClockRadius = 115;

    private readonly RotateTransform _hourRotate;
    private readonly RotateTransform _minuteRotate;
    private readonly RotateTransform _secondRotate;
    private readonly Line _secondHand;
    private readonly System.Windows.Threading.DispatcherTimer _timer;

    public MainWindow()
    {
        InitializeComponent();

        DrawMarkers();
        DrawNumbers();

        (_hourRotate, _) = CreateHand(65, 4.5, new SolidColorBrush(Color.FromArgb(0xDD, 0xFF, 0xFF, 0xFF)));
        (_minuteRotate, _) = CreateHand(85, 3, new SolidColorBrush(Color.FromArgb(0xDD, 0xFF, 0xFF, 0xFF)));
        (_secondRotate, _secondHand) = CreateHand(95, 1.2, new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x35)), tailLength: 20);
        _secondHand.StrokeEndLineCap = PenLineCap.Round;

        var centerCap = new Ellipse
        {
            Width = 8, Height = 8,
            Fill = new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x35)),
        };
        Canvas.SetLeft(centerCap, Center - 4);
        Canvas.SetTop(centerCap, Center - 4);
        ClockCanvas.Children.Add(centerCap);

        _timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();

        SettingsManager.SettingsChanged += OnSettingsChanged;

        UpdateClock();
        Logger.Info("Clock window created");
    }

    private void DrawMarkers()
    {
        for (var i = 0; i < 60; i++)
        {
            var angle = i * 6.0;
            var rad = angle * Math.PI / 180.0;

            if (i % 5 == 0)
            {
                var line = new Line
                {
                    X1 = Center + (ClockRadius - 18) * Math.Sin(rad),
                    Y1 = Center - (ClockRadius - 18) * Math.Cos(rad),
                    X2 = Center + (ClockRadius - 4) * Math.Sin(rad),
                    Y2 = Center - (ClockRadius - 4) * Math.Cos(rad),
                    Stroke = new SolidColorBrush(Color.FromArgb(0xBB, 0xFF, 0xFF, 0xFF)),
                    StrokeThickness = 2,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                };
                ClockCanvas.Children.Add(line);
            }
            else
            {
                var dot = new Ellipse
                {
                    Width = 2, Height = 2,
                    Fill = new SolidColorBrush(Color.FromArgb(0x55, 0xFF, 0xFF, 0xFF)),
                };
                var r = ClockRadius - 10;
                Canvas.SetLeft(dot, Center + r * Math.Sin(rad) - 1);
                Canvas.SetTop(dot, Center - r * Math.Cos(rad) - 1);
                ClockCanvas.Children.Add(dot);
            }
        }
    }

    private void DrawNumbers()
    {
        var numRadius = 88.0;
        for (var i = 1; i <= 12; i++)
        {
            var angle = i * 30.0 * Math.PI / 180.0;
            var text = new TextBlock
            {
                Text = i.ToString(),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromArgb(0xDD, 0xFF, 0xFF, 0xFF)),
                FontFamily = new FontFamily("Segoe UI"),
            };
            text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var x = Center + numRadius * Math.Sin(angle) - text.DesiredSize.Width / 2;
            var y = Center - numRadius * Math.Cos(angle) - text.DesiredSize.Height / 2;
            Canvas.SetLeft(text, x);
            Canvas.SetTop(text, y);
            ClockCanvas.Children.Add(text);
        }
    }

    private (RotateTransform rotate, Line line) CreateHand(double length, double thickness, Brush brush, double tailLength = 0)
    {
        var line = new Line
        {
            X1 = Center,
            Y1 = Center + tailLength,
            X2 = Center,
            Y2 = Center - length,
            Stroke = brush,
            StrokeThickness = thickness,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
        };

        var rotate = new RotateTransform(0, Center, Center);
        line.RenderTransform = rotate;
        ClockCanvas.Children.Add(line);
        return (rotate, line);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        UpdateClock();
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
        var secondAngle = now.Second * 6.0;
        var minuteAngle = now.Minute * 6.0 + now.Second * 0.1;
        var hourAngle = (now.Hour % 12) * 30.0 + now.Minute * 0.5;

        _hourRotate.Angle = hourAngle;
        _minuteRotate.Angle = minuteAngle;
        _secondRotate.Angle = secondAngle;
        _secondHand.Visibility = SettingsManager.Current.ShowSecondHand ? Visibility.Visible : Visibility.Collapsed;

        DigitalTime.Text = SettingsManager.Current.ShowSecondHand
            ? now.ToString("HH:mm:ss")
            : now.ToString("HH:mm");

        DateText.Text = now.ToString("yyyy-MM-dd dddd");
    }

    private void OnSettingsChanged(AppSettings settings)
    {
        UpdateClock();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
        if (Left >= 0 && Top >= 0)
        {
            SettingsManager.SaveWindowPosition(Left, Top);
        }
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
    }

    public void RestorePosition()
    {
        var settings = SettingsManager.Current;
        if (!double.IsNaN(settings.WindowLeft) && !double.IsNaN(settings.WindowTop))
        {
            Left = settings.WindowLeft;
            Top = settings.WindowTop;
        }
        else
        {
            Left = SystemParameters.WorkArea.Width - Width - 20;
            Top = 20;
        }
    }

    private void Menu_Settings_Click(object sender, RoutedEventArgs e)
    {
        var win = new SettingsWindow { Owner = this };
        win.ShowDialog();
    }

    private void Menu_ViewLog_Click(object sender, RoutedEventArgs e)
    {
        var win = new LogWindow { Owner = this };
        win.ShowDialog();
    }

    private void Menu_Exit_Click(object sender, RoutedEventArgs e)
    {
        Logger.Info("Application exiting from menu");
        System.Windows.Application.Current.Shutdown();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer.Stop();
        base.OnClosed(e);
    }
}

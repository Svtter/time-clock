using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TimeClock.Helpers;

/// <summary>
/// Samples the screen content behind the clock's text area so the digital
/// time and date can switch between light and dark text for readability.
/// </summary>
internal static class BackgroundContrast
{
    private const double LightThreshold = 0.6;
    private const int SampleStep = 4;

    private const int SM_XVIRTUALSCREEN = 76;
    private const int SM_YVIRTUALSCREEN = 77;
    private const int SM_CXVIRTUALSCREEN = 78;
    private const int SM_CYVIRTUALSCREEN = 79;

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    /// <summary>
    /// Captures a physical-screen rectangle and reports whether its content is light.
    /// Returns null when the capture fails or the rectangle is off-screen, so the
    /// caller can keep its current text color.
    /// </summary>
    public static bool? IsLightBackground(Rectangle screenRect)
    {
        var clamped = ClampToVirtualScreen(screenRect);
        if (clamped.Width <= 0 || clamped.Height <= 0)
        {
            return null;
        }

        try
        {
            using var bitmap = new Bitmap(clamped.Width, clamped.Height, PixelFormat.Format32bppRgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(clamped.X, clamped.Y, 0, 0,
                    new System.Drawing.Size(clamped.Width, clamped.Height));
            }

            var luminances = new List<double>(capacity: 256);
            for (var y = 0; y < clamped.Height; y += SampleStep)
            {
                for (var x = 0; x < clamped.Width; x += SampleStep)
                {
                    var c = bitmap.GetPixel(x, y);
                    luminances.Add((0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255.0);
                }
            }

            if (luminances.Count == 0)
            {
                return null;
            }

            // The text itself is drawn over the sampled area; the median ignores it
            // as long as glyphs cover less than half of the rectangle.
            luminances.Sort();
            var median = luminances[luminances.Count / 2];
            return median > LightThreshold;
        }
        catch
        {
            return null;
        }
    }

    private static Rectangle ClampToVirtualScreen(Rectangle rect)
    {
        var virtualRect = new Rectangle(
            GetSystemMetrics(SM_XVIRTUALSCREEN),
            GetSystemMetrics(SM_YVIRTUALSCREEN),
            GetSystemMetrics(SM_CXVIRTUALSCREEN),
            GetSystemMetrics(SM_CYVIRTUALSCREEN));
        return Rectangle.Intersect(rect, virtualRect);
    }
}

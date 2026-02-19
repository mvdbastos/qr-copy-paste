using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace QrCopyPaste;

/// <summary>
/// Service for capturing screen regions on Windows.
/// Uses P/Invoke for window management as these APIs are not available in managed code.
/// </summary>
public class ScreenCaptureService
{
    // P/Invoke is necessary for Windows window management APIs not available in .NET
    [DllImport("user32.dll")]
    [SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", 
        Justification = "Windows API required for window detection")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible",
        Justification = "Windows API required for window bounds")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    [SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible",
        Justification = "Windows API required for window title")]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    /// <summary>
    /// Captures a specific region of the screen.
    /// </summary>
    public Bitmap CaptureRegion(Rectangle region)
    {
        var bitmap = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(region.Left, region.Top, 0, 0, region.Size);
        }
        return bitmap;
    }

    /// <summary>
    /// Captures the active window.
    /// </summary>
    public Bitmap? CaptureActiveWindow()
    {
        IntPtr handle = GetForegroundWindow();
        if (handle == IntPtr.Zero)
            return null;

        if (!GetWindowRect(handle, out RECT rect))
            return null;

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        if (width <= 0 || height <= 0)
            return null;

        return CaptureRegion(new Rectangle(rect.Left, rect.Top, width, height));
    }

    /// <summary>
    /// Gets the active window title.
    /// </summary>
    public string GetActiveWindowTitle()
    {
        IntPtr handle = GetForegroundWindow();
        if (handle == IntPtr.Zero)
            return string.Empty;

        var text = new System.Text.StringBuilder(256);
        GetWindowText(handle, text, text.Capacity);
        return text.ToString();
    }

    /// <summary>
    /// Captures the entire screen (all monitors).
    /// </summary>
    public Bitmap CaptureAllScreens()
    {
        var bounds = SystemInformation.VirtualScreen;
        return CaptureRegion(bounds);
    }

    /// <summary>
    /// Captures the active monitor.
    /// </summary>
    public Bitmap CaptureActiveMonitor()
    {
        var cursorPosition = Cursor.Position;
        var activeScreen = Screen.FromPoint(cursorPosition);
        return CaptureRegion(activeScreen.Bounds);
    }
}

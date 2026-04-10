using System.Drawing;

namespace QrCopyPaste;

/// <summary>
/// Background service for automatically scanning QR codes from the screen.
/// </summary>
public class AutoScanService : IDisposable
{
    private readonly System.Threading.Timer scanTimer;
    private readonly ScreenCaptureService captureService;
    private readonly QrDecoderService decoderService;
    private readonly AppSettings settings;
    private readonly Action<string, string?> onQrDetected;
    
    private string? lastDetectedQr;
    private bool isThrottled;
    private int isScanning;  // 0 = not scanning, 1 = scanning (used with Interlocked)

    public bool IsRunning { get; private set; }

    public AutoScanService(
        AppSettings settings,
        ScreenCaptureService captureService,
        QrDecoderService decoderService,
        Action<string, string?> onQrDetected)
    {
        this.settings = settings;
        this.captureService = captureService;
        this.decoderService = decoderService;
        this.onQrDetected = onQrDetected;

        // Calculate interval based on FPS cap (e.g., 3 FPS = 333ms interval)
        scanTimer = new System.Threading.Timer(ScanCallback, null, Timeout.Infinite, NormalIntervalMs);
    }

    public void Start()
    {
        if (!IsRunning)
        {
            IsRunning = true;
            scanTimer.Change(0, NormalIntervalMs);
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
            scanTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    private void ScanCallback(object? state)
    {
        // Prevent overlapping scans using thread-safe Interlocked operation
        // If already scanning (value is 1), return without changing it
        if (Interlocked.CompareExchange(ref isScanning, 1, 0) != 0)
            return;

        try
        {
            PerformScan();
        }
        finally
        {
            // Reset scanning flag
            Interlocked.Exchange(ref isScanning, 0);
        }
    }

    private void PerformScan()
    {
        try
        {
            // Get the active window title for blocking check
            var windowTitle = captureService.GetActiveWindowTitle();
            
            // Check if window is blocked
            if (!string.IsNullOrEmpty(windowTitle) && 
                settings.BlockedWindowTitles.Any(blocked => windowTitle.Contains(blocked, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            // Capture based on scope
            Bitmap? capture = settings.AutoScanScope switch
            {
                AutoScanScope.ActiveWindow => captureService.CaptureActiveWindow(),
                AutoScanScope.ActiveMonitor => captureService.CaptureActiveMonitor(),
                AutoScanScope.AllMonitors => captureService.CaptureAllScreens(),
                _ => captureService.CaptureActiveWindow()
            };

            if (capture == null)
                return;

            using (capture)
            {
                // Decode QR code once per capture
                var decodedText = decoderService.DecodeQrCode(capture);

                if (string.IsNullOrWhiteSpace(decodedText))
                {
                    // No QR found - reset so the same code can be re-detected after it disappears
                    lastDetectedQr = null;
                    RestoreNormalScanRate();
                    return;
                }

                // Deduplicate - skip if this QR was already handled
                if (decodedText == lastDetectedQr)
                {
                    // Throttle down scan rate while the same QR stays on screen
                    ThrottleScanRate();
                    return;
                }

                // New QR code detected - restore normal rate and notify
                lastDetectedQr = decodedText;
                RestoreNormalScanRate();

                // Notify detection on UI thread
                onQrDetected?.Invoke(decodedText, windowTitle);
            }
        }
        catch
        {
            // Silently ignore scanning errors to prevent spam
        }
    }

    private int NormalIntervalMs => 1000 / Math.Max(1, settings.AutoScanFpsCap);

    private void ThrottleScanRate()
    {
        if (!isThrottled)
        {
            isThrottled = true;
            // Use DebounceSeconds (converted to ms) as the throttle interval to reduce CPU usage
            const int msPerSecond = 1000;
            var throttledIntervalMs = Math.Max(NormalIntervalMs, settings.DebounceSeconds * msPerSecond);
            scanTimer.Change(throttledIntervalMs, throttledIntervalMs);
        }
    }

    private void RestoreNormalScanRate()
    {
        if (isThrottled)
        {
            isThrottled = false;
            scanTimer.Change(NormalIntervalMs, NormalIntervalMs);
        }
    }

    public void Dispose()
    {
        Stop();
        scanTimer?.Dispose();
    }
}

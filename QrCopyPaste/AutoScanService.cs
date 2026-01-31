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
    private DateTime lastDetectionTime = DateTime.MinValue;
    private bool isScanning;

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
        var intervalMs = 1000 / Math.Max(1, settings.AutoScanFpsCap);
        scanTimer = new System.Threading.Timer(ScanCallback, null, Timeout.Infinite, intervalMs);
    }

    public void Start()
    {
        if (!IsRunning)
        {
            IsRunning = true;
            var intervalMs = 1000 / Math.Max(1, settings.AutoScanFpsCap);
            scanTimer.Change(0, intervalMs);
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
        // Prevent overlapping scans
        if (isScanning)
            return;

        isScanning = true;
        try
        {
            PerformScan();
        }
        finally
        {
            isScanning = false;
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
                // Check if QR meets minimum size requirement
                if (!decoderService.ContainsQrCode(capture, settings.MinQrSize))
                    return;

                // Decode QR code
                var decodedText = decoderService.DecodeQrCode(capture);
                if (string.IsNullOrWhiteSpace(decodedText))
                    return;

                // Check debounce - don't re-detect same QR within debounce period
                if (decodedText == lastDetectedQr &&
                    lastDetectionTime != DateTime.MinValue &&
                    (DateTime.Now - lastDetectionTime).TotalSeconds < settings.DebounceSeconds)
                {
                    return;
                }

                // Update last detection
                lastDetectedQr = decodedText;
                lastDetectionTime = DateTime.Now;

                // Notify detection on UI thread
                onQrDetected?.Invoke(decodedText, windowTitle);
            }
        }
        catch
        {
            // Silently ignore scanning errors to prevent spam
        }
    }

    public void Dispose()
    {
        Stop();
        scanTimer?.Dispose();
    }
}

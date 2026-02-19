namespace QrCopyPaste;

/// <summary>
/// Defines the scope for auto-scanning QR codes.
/// </summary>
public enum AutoScanScope
{
    /// <summary>
    /// Scan only the active window.
    /// </summary>
    ActiveWindow,

    /// <summary>
    /// Scan the entire active monitor.
    /// </summary>
    ActiveMonitor,

    /// <summary>
    /// Scan all monitors.
    /// </summary>
    AllMonitors
}

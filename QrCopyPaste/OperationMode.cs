namespace QrCopyPaste;

/// <summary>
/// Defines the operation modes for the application.
/// </summary>
public enum OperationMode
{
    /// <summary>
    /// Clipboard → QR: Generate and display QR codes from clipboard text.
    /// </summary>
    ClipboardToQr,

    /// <summary>
    /// QR → Clipboard: Scan QR codes from screen and inject into clipboard.
    /// </summary>
    QrToClipboard
}

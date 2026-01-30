using System.Drawing;
using System.Windows.Forms;
using QRCoder;

namespace QrCopyPaste;

public class QrOverlay : Form
{
    private readonly System.Windows.Forms.Timer autoCloseTimer;
    private readonly int dismissSeconds;
    private const int MinPixelsPerModule = 2; // Minimum 2x2 pixels per QR module to be distinguishable
    private const int Padding = 20;

    public QrOverlay(string text, int dismissSeconds)
    {
        this.dismissSeconds = dismissSeconds;

        // Get the active monitor to determine max size
        var cursorPosition = Cursor.Position;
        var activeScreen = Screen.FromPoint(cursorPosition);
        
        // Calculate maximum QR size as 1/8 of screen (considering both width and height)
        var maxScreenDimension = Math.Min(activeScreen.Bounds.Width, activeScreen.Bounds.Height);
        var maxQrSize = maxScreenDimension / 8;

        // Generate QR code data first to determine module count
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        
        // Calculate the optimal pixels per module
        var moduleCount = qrCodeData.ModuleMatrix.Count;
        var pixelsPerModule = CalculatePixelsPerModule(moduleCount, maxQrSize);
        
        // Generate QR code bitmap with calculated size
        using var qrCode = new QRCode(qrCodeData);
        var qrBitmap = qrCode.GetGraphic(pixelsPerModule);

        // Configure form
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        BackColor = Color.White;
        Size = new Size(qrBitmap.Width + Padding, qrBitmap.Height + Padding);

        // Create PictureBox for QR code - PictureBox takes ownership of the image
        var pictureBox = new PictureBox
        {
            Image = qrBitmap,
            SizeMode = PictureBoxSizeMode.CenterImage,
            Dock = DockStyle.Fill
        };
        Controls.Add(pictureBox);

        // Position at bottom-right of active monitor
        PositionOnActiveMonitor();

        // Auto-close timer
        autoCloseTimer = new System.Windows.Forms.Timer { Interval = dismissSeconds * 1000 };
        autoCloseTimer.Tick += (s, e) =>
        {
            autoCloseTimer.Stop();
            Close();
        };
        autoCloseTimer.Start();

        // Click to close
        pictureBox.Click += (s, e) => Close();
        Click += (s, e) => Close();
    }

    private int CalculatePixelsPerModule(int moduleCount, int maxQrSize)
    {
        // Calculate maximum pixels per module that fits within maxQrSize
        var maxPixelsPerModule = maxQrSize / moduleCount;
        
        // Use at least MinPixelsPerModule to keep modules distinguishable
        // Use at most maxPixelsPerModule to fit within screen constraint
        var pixelsPerModule = Math.Max(MinPixelsPerModule, Math.Min(maxPixelsPerModule, 20));
        
        return pixelsPerModule;
    }

    private void PositionOnActiveMonitor()
    {
        // Get the active monitor (where the cursor is)
        var cursorPosition = Cursor.Position;
        var activeScreen = Screen.FromPoint(cursorPosition);

        // Position at bottom-right with padding
        Location = new Point(
            activeScreen.WorkingArea.Right - Width - Padding,
            activeScreen.WorkingArea.Bottom - Height - Padding
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            autoCloseTimer?.Stop();
            autoCloseTimer?.Dispose();
            // PictureBox will dispose its image automatically
        }
        base.Dispose(disposing);
    }
}

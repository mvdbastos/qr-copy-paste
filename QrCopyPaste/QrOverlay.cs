using System.Drawing;
using System.Windows.Forms;
using QRCoder;

namespace QrCopyPaste;

public class QrOverlay : Form
{
    private readonly System.Windows.Forms.Timer autoCloseTimer;
    private readonly int dismissSeconds;
    private Bitmap? qrBitmap;

    public QrOverlay(string text, int dismissSeconds)
    {
        this.dismissSeconds = dismissSeconds;

        // Generate QR code
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCode(qrCodeData);
        qrBitmap = qrCode.GetGraphic(20);

        // Configure form
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        BackColor = Color.White;
        Size = new Size(qrBitmap.Width + 20, qrBitmap.Height + 20);

        // Create PictureBox for QR code
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

    private void PositionOnActiveMonitor()
    {
        // Get the active monitor (where the cursor is)
        var cursorPosition = Cursor.Position;
        var activeScreen = Screen.FromPoint(cursorPosition);

        // Position at bottom-right with padding
        const int padding = 20;
        Location = new Point(
            activeScreen.WorkingArea.Right - Width - padding,
            activeScreen.WorkingArea.Bottom - Height - padding
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            autoCloseTimer?.Stop();
            autoCloseTimer?.Dispose();
            qrBitmap?.Dispose();
        }
        base.Dispose(disposing);
    }
}

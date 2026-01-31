using System.Drawing;
using System.Windows.Forms;
using QRCoder;

namespace QrCopyPaste;

public class QrOverlay : Form
{
    private readonly System.Windows.Forms.Timer autoCloseTimer;
    private readonly int dismissSeconds;
    private const int MinPixelsPerModule = 2;
    private const int Padding = 20;
    private const int MessageBoxSize = 200;
    private const int ButtonHeight = 30;
    private const int ButtonSpacing = 10;
    private Font? messageFont;
    private Bitmap? qrBitmap;
    private string? qrText;

    public QrOverlay(string? text, int dismissSeconds, QrErrorCorrectionLevel errorCorrectionLevel = QrErrorCorrectionLevel.Low)
    {
        this.dismissSeconds = dismissSeconds;
        this.qrText = text;

        // Get the active monitor to determine max size
        var cursorPosition = Cursor.Position;
        var activeScreen = Screen.FromPoint(cursorPosition);

        // Check if text is empty or null
        if (string.IsNullOrWhiteSpace(text))
        {
            CreateMessageOverlay("NO TEXT FOUND");
        }
        else
        {
            CreateQrCodeOverlay(text, activeScreen, errorCorrectionLevel);
        }

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
        Click += (s, e) => Close();
    }

    private void CreateMessageOverlay(string message)
    {
        // Configure form for message
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        BackColor = Color.White;
        Size = new Size(MessageBoxSize + Padding, MessageBoxSize + Padding);

        // Create font for label (will be disposed in Dispose method)
        messageFont = new Font("Segoe UI", 12, FontStyle.Bold);

        // Create label for message
        var label = new Label
        {
            Text = message,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = messageFont,
            ForeColor = Color.DarkRed,
            BackColor = Color.White
        };
        Controls.Add(label);

        // Click to close
        label.Click += (s, e) => Close();
    }

    private void CreateQrCodeOverlay(string text, Screen activeScreen, QrErrorCorrectionLevel errorCorrectionLevel)
    {
        // Calculate maximum QR size as 1/8 of screen (considering both width and height)
        var maxScreenDimension = Math.Min(activeScreen.Bounds.Width, activeScreen.Bounds.Height);
        var maxQrSize = maxScreenDimension / 8;

        // Map our error correction level to QRCoder's ECCLevel
        var eccLevel = errorCorrectionLevel switch
        {
            QrErrorCorrectionLevel.Low => QRCodeGenerator.ECCLevel.L,
            QrErrorCorrectionLevel.Medium => QRCodeGenerator.ECCLevel.M,
            QrErrorCorrectionLevel.Quartile => QRCodeGenerator.ECCLevel.Q,
            QrErrorCorrectionLevel.High => QRCodeGenerator.ECCLevel.H,
            _ => QRCodeGenerator.ECCLevel.L
        };

        // Generate QR code data first to determine module count
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(
            plainText: text, eccLevel: eccLevel,
            forceUtf8: true,
            utf8BOM: true,
            eciMode: QRCodeGenerator.EciMode.Utf8
        );
        
        // Calculate the optimal pixels per module
        var moduleCount = qrCodeData.ModuleMatrix.Count;
        var pixelsPerModule = CalculatePixelsPerModule(moduleCount, maxQrSize);
        
        // Generate QR code bitmap with calculated size
        using var qrCode = new QRCode(qrCodeData);
        qrBitmap = qrCode.GetGraphic(pixelsPerModule);

        // Configure form
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        BackColor = Color.White;

        // Create panel for QR code
        var qrPanel = new Panel
        {
            Size = new Size(qrBitmap.Width + Padding, qrBitmap.Height + Padding),
            Location = new Point(0, 0),
            BackColor = Color.White
        };

        // Create PictureBox for QR code - give it a copy to avoid double-disposal
        var pictureBox = new PictureBox
        {
            Image = (Bitmap)qrBitmap.Clone(),
            SizeMode = PictureBoxSizeMode.CenterImage,
            Dock = DockStyle.Fill
        };
        qrPanel.Controls.Add(pictureBox);
        pictureBox.Click += (s, e) => Close();

        // Create button panel at bottom
        var buttonPanel = new Panel
        {
            Height = ButtonHeight + ButtonSpacing * 2,
            Dock = DockStyle.Bottom,
            BackColor = Color.White
        };

        // Copy Image button
        var btnCopyImage = new Button
        {
            Text = "Copy Image",
            Width = 100,
            Height = ButtonHeight,
            Location = new Point(ButtonSpacing, ButtonSpacing)
        };
        btnCopyImage.Click += (s, e) => CopyImageToClipboard();
        buttonPanel.Controls.Add(btnCopyImage);

        // Save button
        var btnSave = new Button
        {
            Text = "Save...",
            Width = 80,
            Height = ButtonHeight,
            Location = new Point(btnCopyImage.Right + ButtonSpacing, ButtonSpacing)
        };
        btnSave.Click += (s, e) => SaveQrImage();
        buttonPanel.Controls.Add(btnSave);

        // Set form size
        Size = new Size(qrPanel.Width, qrPanel.Height + buttonPanel.Height);

        // Add controls to form
        Controls.Add(qrPanel);
        Controls.Add(buttonPanel);
    }

    private void CopyImageToClipboard()
    {
        if (qrBitmap != null)
        {
            // Stop auto-dismiss timer while showing notification
            autoCloseTimer?.Stop();
            
            try
            {
                Clipboard.SetImage(qrBitmap);
                // Close overlay and let tray notification show
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void SaveQrImage()
    {
        if (qrBitmap != null)
        {
            // Stop auto-dismiss timer while dialog is open
            autoCloseTimer?.Stop();
            
            using var saveDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save QR Code",
                FileName = "qrcode.png"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    qrBitmap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    // Close overlay after successful save
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // User cancelled - restart the timer
                autoCloseTimer?.Start();
            }
        }
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
            messageFont?.Dispose();
            qrBitmap?.Dispose();
        }
        base.Dispose(disposing);
    }
}

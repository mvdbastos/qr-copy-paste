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
    private const int ButtonSpacing = 5;
    private Font? messageFont;
    private Bitmap? qrBitmap;

    public QrOverlay(string? text, int dismissSeconds, QRCodeGenerator.ECCLevel eccLevel = QRCodeGenerator.ECCLevel.L)
    {
        this.dismissSeconds = dismissSeconds;

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
            CreateQrCodeOverlay(text, activeScreen, eccLevel);
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

    private void CreateQrCodeOverlay(string text, Screen activeScreen, QRCodeGenerator.ECCLevel eccLevel)
    {
        // Calculate maximum QR size as 1/8 of screen (considering both width and height)
        var maxScreenDimension = Math.Min(activeScreen.Bounds.Width, activeScreen.Bounds.Height);
        var maxQrSize = maxScreenDimension / 8;

        // Generate QR code data first to determine module count
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(
            plainText: text,
            eccLevel: eccLevel,
            forceUtf8: true,
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
        
        // Add buttons panel at bottom
        var buttonPanel = new Panel
        {
            Height = ButtonHeight + ButtonSpacing * 2,
            Dock = DockStyle.Bottom,
            BackColor = Color.LightGray
        };

        var btnCopy = new Button
        {
            Text = "Copy Image",
            Width = 100,
            Height = ButtonHeight,
            Left = ButtonSpacing,
            Top = ButtonSpacing
        };
        btnCopy.Click += (s, e) => CopyQrImageToClipboard();
        buttonPanel.Controls.Add(btnCopy);

        var btnSave = new Button
        {
            Text = "Save As...",
            Width = 100,
            Height = ButtonHeight,
            Left = btnCopy.Right + ButtonSpacing,
            Top = ButtonSpacing
        };
        btnSave.Click += (s, e) => SaveQrImage();
        buttonPanel.Controls.Add(btnSave);

        Controls.Add(buttonPanel);

        // Create container panel for QR code with padding
        var qrContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(Padding)
        };

        // Create PictureBox for QR code
        var pictureBox = new PictureBox
        {
            Image = qrBitmap,
            SizeMode = PictureBoxSizeMode.AutoSize,
            Dock = DockStyle.None,
            BackColor = Color.White,
            Left = Padding,
            Top = Padding
        };
        qrContainer.Controls.Add(pictureBox);
        Controls.Add(qrContainer);
        
        // Calculate form size properly with padding around QR code
        var formWidth = qrBitmap.Width + Padding * 2;
        var formHeight = qrBitmap.Height + Padding * 2 + buttonPanel.Height;
        
        Size = new Size(formWidth, formHeight);

        // Click to close
        pictureBox.Click += (s, e) => Close();
        qrContainer.Click += (s, e) => Close();
    }

    private void CopyQrImageToClipboard()
    {
        if (qrBitmap != null)
        {
            try
            {
                Clipboard.SetImage(qrBitmap);
                MessageBox.Show("QR code image copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            using var saveDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp",
                DefaultExt = "png",
                FileName = $"qr_code_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var format = Path.GetExtension(saveDialog.FileName).ToLower() switch
                    {
                        ".jpg" or ".jpeg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                        ".bmp" => System.Drawing.Imaging.ImageFormat.Bmp,
                        _ => System.Drawing.Imaging.ImageFormat.Png
                    };

                    qrBitmap.Save(saveDialog.FileName, format);
                    MessageBox.Show($"QR code saved to {saveDialog.FileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

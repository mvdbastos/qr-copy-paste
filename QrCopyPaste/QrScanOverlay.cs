using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace QrCopyPaste;

public class QrScanOverlay : Form
{
    private Point startPoint;
    private Rectangle selectionRect;
    private bool isSelecting = false;
    public string? DecodedText { get; private set; }

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
        IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hDC);

    private const int SRCCOPY = 0x00CC0020;

    public QrScanOverlay()
    {
        // Set up full screen overlay
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        Cursor = Cursors.Cross;
        BackColor = Color.Black;
        Opacity = 0.3;
        DoubleBuffered = true;

        // Event handlers
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
        Paint += OnPaint;
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isSelecting = true;
            startPoint = e.Location;
            selectionRect = new Rectangle(e.Location, Size.Empty);
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (isSelecting)
        {
            selectionRect = new Rectangle(
                Math.Min(startPoint.X, e.X),
                Math.Min(startPoint.Y, e.Y),
                Math.Abs(e.X - startPoint.X),
                Math.Abs(e.Y - startPoint.Y)
            );
            Invalidate();
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && isSelecting)
        {
            isSelecting = false;

            if (selectionRect.Width > 10 && selectionRect.Height > 10)
            {
                try
                {
                    // Capture the selected region
                    using var bitmap = CaptureScreen(selectionRect);
                    
                    // Decode QR code
                    DecodedText = DecodeQrCode(bitmap);

                    if (!string.IsNullOrEmpty(DecodedText))
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("No QR code found in the selected region.", "QR Scan", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        selectionRect = Rectangle.Empty;
                        Invalidate();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error scanning QR code: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            }
        }
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        if (!selectionRect.IsEmpty)
        {
            // Draw selection rectangle
            using var pen = new Pen(Color.Red, 2);
            e.Graphics.DrawRectangle(pen, selectionRect);

            // Draw semi-transparent fill
            using var brush = new SolidBrush(Color.FromArgb(50, Color.White));
            e.Graphics.FillRectangle(brush, selectionRect);
        }
    }

    private Bitmap CaptureScreen(Rectangle region)
    {
        var desktopHandle = GetDesktopWindow();
        var desktopDC = GetWindowDC(desktopHandle);
        var memoryDC = CreateCompatibleDC(desktopDC);
        var bitmap = CreateCompatibleBitmap(desktopDC, region.Width, region.Height);
        var oldBitmap = SelectObject(memoryDC, bitmap);

        BitBlt(memoryDC, 0, 0, region.Width, region.Height, desktopDC, region.X, region.Y, SRCCOPY);

        SelectObject(memoryDC, oldBitmap);
        DeleteDC(memoryDC);
        ReleaseDC(desktopHandle, desktopDC);

        var image = Image.FromHbitmap(bitmap);
        DeleteObject(bitmap);

        return image;
    }

    private string? DecodeQrCode(Bitmap bitmap)
    {
        var reader = new BarcodeReader
        {
            AutoRotate = true,
            TryInverted = true,
            Options = new DecodingOptions
            {
                PossibleFormats = new[] { BarcodeFormat.QR_CODE },
                TryHarder = true
            }
        };

        var result = reader.Decode(bitmap);
        return result?.Text;
    }
}

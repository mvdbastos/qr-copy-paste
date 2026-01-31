using System.Drawing;
using System.Windows.Forms;

namespace QrCopyPaste;

/// <summary>
/// Overlay form that allows users to select a region of the screen for QR scanning.
/// </summary>
public class RegionSelectionOverlay : Form
{
    private Point startPoint;
    private Point currentPoint;
    private bool isDrawing;
    private Bitmap? screenCapture;
    public Rectangle SelectedRegion { get; private set; }
    public bool RegionSelected { get; private set; }

    public RegionSelectionOverlay()
    {
        // Capture the screen before showing overlay
        CaptureScreen();

        // Configure form
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        ShowInTaskbar = false;
        DoubleBuffered = true;
        Cursor = Cursors.Cross;
        
        // Semi-transparent dark overlay
        BackColor = Color.Black;
        Opacity = 0.3;

        // Handle mouse events
        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
        KeyDown += OnKeyDown;
        Paint += OnPaint;
    }

    private void CaptureScreen()
    {
        try
        {
            var bounds = SystemInformation.VirtualScreen;
            screenCapture = new Bitmap(bounds.Width, bounds.Height);
            using (var graphics = Graphics.FromImage(screenCapture))
            {
                graphics.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
            }
        }
        catch
        {
            // If screen capture fails, continue without background
        }
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isDrawing = true;
            startPoint = e.Location;
            currentPoint = e.Location;
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (isDrawing)
        {
            currentPoint = e.Location;
            Invalidate(); // Trigger repaint
        }
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && isDrawing)
        {
            isDrawing = false;
            
            // Calculate the selected region
            var x = Math.Min(startPoint.X, currentPoint.X);
            var y = Math.Min(startPoint.Y, currentPoint.Y);
            var width = Math.Abs(currentPoint.X - startPoint.X);
            var height = Math.Abs(currentPoint.Y - startPoint.Y);

            if (width > 10 && height > 10) // Minimum size threshold
            {
                SelectedRegion = new Rectangle(x, y, width, height);
                RegionSelected = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                // Selection too small, cancel
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    private void OnPaint(object? sender, PaintEventArgs e)
    {
        if (isDrawing)
        {
            // Draw selection rectangle
            var x = Math.Min(startPoint.X, currentPoint.X);
            var y = Math.Min(startPoint.Y, currentPoint.Y);
            var width = Math.Abs(currentPoint.X - startPoint.X);
            var height = Math.Abs(currentPoint.Y - startPoint.Y);

            using (var pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, x, y, width, height);
            }

            // Draw dimension text
            using (var brush = new SolidBrush(Color.White))
            using (var font = new Font("Segoe UI", 12))
            {
                var text = $"{width} x {height}";
                var textSize = e.Graphics.MeasureString(text, font);
                var textX = x + width / 2 - textSize.Width / 2;
                var textY = y + height / 2 - textSize.Height / 2;
                e.Graphics.DrawString(text, font, brush, textX, textY);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            screenCapture?.Dispose();
        }
        base.Dispose(disposing);
    }
}

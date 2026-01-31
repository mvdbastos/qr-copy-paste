namespace QrCopyPaste;

partial class MainForm
{
    private readonly System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            RemoveClipboardFormatListener(Handle);
            UnregisterGlobalHotKey();
            trayContextMenu?.Dispose();
            trayIcon?.Dispose();
            currentOverlay?.Dispose();
            if (trayIconHandle != IntPtr.Zero)
            {
                DestroyIcon(trayIconHandle);
            }
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(0, 0);
        FormBorderStyle = FormBorderStyle.None;
        Name = "MainForm";
        Text = "QR Copy-Paste";
        WindowState = FormWindowState.Minimized;
        ResumeLayout(false);
    }
}

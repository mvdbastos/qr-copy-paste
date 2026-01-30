namespace QrCopyPaste;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        
        // MainForm
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(0, 0);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.Name = "MainForm";
        this.Text = "QR Copy-Paste";
        this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
        this.ResumeLayout(false);
    }
}

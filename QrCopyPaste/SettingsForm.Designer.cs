namespace QrCopyPaste;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Label lblAutoDismiss;
    private System.Windows.Forms.NumericUpDown numAutoDismiss;
    private System.Windows.Forms.Label lblMaxLength;
    private System.Windows.Forms.NumericUpDown numMaxLength;
    private System.Windows.Forms.Label lblThrottle;
    private System.Windows.Forms.NumericUpDown numThrottle;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;

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
        this.lblAutoDismiss = new System.Windows.Forms.Label();
        this.numAutoDismiss = new System.Windows.Forms.NumericUpDown();
        this.lblMaxLength = new System.Windows.Forms.Label();
        this.numMaxLength = new System.Windows.Forms.NumericUpDown();
        this.lblThrottle = new System.Windows.Forms.Label();
        this.numThrottle = new System.Windows.Forms.NumericUpDown();
        this.btnSave = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.numAutoDismiss)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numMaxLength)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numThrottle)).BeginInit();
        this.SuspendLayout();
        
        // lblAutoDismiss
        this.lblAutoDismiss.AutoSize = true;
        this.lblAutoDismiss.Location = new System.Drawing.Point(20, 20);
        this.lblAutoDismiss.Name = "lblAutoDismiss";
        this.lblAutoDismiss.Size = new System.Drawing.Size(150, 15);
        this.lblAutoDismiss.Text = "Auto-dismiss (seconds):";
        
        // numAutoDismiss
        this.numAutoDismiss.Location = new System.Drawing.Point(200, 18);
        this.numAutoDismiss.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        this.numAutoDismiss.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
        this.numAutoDismiss.Name = "numAutoDismiss";
        this.numAutoDismiss.Size = new System.Drawing.Size(100, 23);
        this.numAutoDismiss.Value = new decimal(new int[] { 5, 0, 0, 0 });
        
        // lblMaxLength
        this.lblMaxLength.AutoSize = true;
        this.lblMaxLength.Location = new System.Drawing.Point(20, 55);
        this.lblMaxLength.Name = "lblMaxLength";
        this.lblMaxLength.Size = new System.Drawing.Size(170, 15);
        this.lblMaxLength.Text = "Max text length (characters):";
        
        // numMaxLength
        this.numMaxLength.Location = new System.Drawing.Point(200, 53);
        this.numMaxLength.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
        this.numMaxLength.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
        this.numMaxLength.Increment = new decimal(new int[] { 50, 0, 0, 0 });
        this.numMaxLength.Name = "numMaxLength";
        this.numMaxLength.Size = new System.Drawing.Size(100, 23);
        this.numMaxLength.Value = new decimal(new int[] { 500, 0, 0, 0 });
        
        // lblThrottle
        this.lblThrottle.AutoSize = true;
        this.lblThrottle.Location = new System.Drawing.Point(20, 90);
        this.lblThrottle.Name = "lblThrottle";
        this.lblThrottle.Size = new System.Drawing.Size(170, 15);
        this.lblThrottle.Text = "Throttle delay (milliseconds):";
        
        // numThrottle
        this.numThrottle.Location = new System.Drawing.Point(200, 88);
        this.numThrottle.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        this.numThrottle.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
        this.numThrottle.Increment = new decimal(new int[] { 100, 0, 0, 0 });
        this.numThrottle.Name = "numThrottle";
        this.numThrottle.Size = new System.Drawing.Size(100, 23);
        this.numThrottle.Value = new decimal(new int[] { 500, 0, 0, 0 });
        
        // btnSave
        this.btnSave.Location = new System.Drawing.Point(120, 130);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(80, 30);
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
        
        // btnCancel
        this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.btnCancel.Location = new System.Drawing.Point(210, 130);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(80, 30);
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        
        // SettingsForm
        this.AcceptButton = this.btnSave;
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(330, 180);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.numThrottle);
        this.Controls.Add(this.lblThrottle);
        this.Controls.Add(this.numMaxLength);
        this.Controls.Add(this.lblMaxLength);
        this.Controls.Add(this.numAutoDismiss);
        this.Controls.Add(this.lblAutoDismiss);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "SettingsForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "QR Copy-Paste Settings";
        ((System.ComponentModel.ISupportInitialize)(this.numAutoDismiss)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numMaxLength)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numThrottle)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}

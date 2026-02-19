namespace QrCopyPaste;

partial class SettingsForm
{
    private readonly System.ComponentModel.IContainer components = null;
    
    // Clipboard → QR controls
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tabClipboardToQr;
    private System.Windows.Forms.TabPage tabQrToClipboard;
    private System.Windows.Forms.Label lblAutoDismiss;
    private System.Windows.Forms.NumericUpDown numAutoDismiss;
    private System.Windows.Forms.Label lblMaxLength;
    private System.Windows.Forms.NumericUpDown numMaxLength;
    private System.Windows.Forms.Label lblThrottle;
    private System.Windows.Forms.NumericUpDown numThrottle;
    private System.Windows.Forms.Label lblErrorCorrection;
    private System.Windows.Forms.ComboBox cmbErrorCorrection;
    
    // QR → Clipboard controls
    private System.Windows.Forms.CheckBox chkAutoScan;
    private System.Windows.Forms.Label lblAutoScanScope;
    private System.Windows.Forms.ComboBox cmbAutoScanScope;
    private System.Windows.Forms.Label lblFpsCap;
    private System.Windows.Forms.NumericUpDown numFpsCap;
    private System.Windows.Forms.Label lblMinQrSize;
    private System.Windows.Forms.NumericUpDown numMinQrSize;
    private System.Windows.Forms.Label lblDebounce;
    private System.Windows.Forms.NumericUpDown numDebounce;
    private System.Windows.Forms.CheckBox chkConfirmSensitive;
    
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
        this.tabControl = new System.Windows.Forms.TabControl();
        this.tabClipboardToQr = new System.Windows.Forms.TabPage();
        this.tabQrToClipboard = new System.Windows.Forms.TabPage();
        
        // Clipboard → QR controls
        this.lblAutoDismiss = new System.Windows.Forms.Label();
        this.numAutoDismiss = new System.Windows.Forms.NumericUpDown();
        this.lblMaxLength = new System.Windows.Forms.Label();
        this.numMaxLength = new System.Windows.Forms.NumericUpDown();
        this.lblThrottle = new System.Windows.Forms.Label();
        this.numThrottle = new System.Windows.Forms.NumericUpDown();
        this.lblErrorCorrection = new System.Windows.Forms.Label();
        this.cmbErrorCorrection = new System.Windows.Forms.ComboBox();
        
        // QR → Clipboard controls
        this.chkAutoScan = new System.Windows.Forms.CheckBox();
        this.lblAutoScanScope = new System.Windows.Forms.Label();
        this.cmbAutoScanScope = new System.Windows.Forms.ComboBox();
        this.lblFpsCap = new System.Windows.Forms.Label();
        this.numFpsCap = new System.Windows.Forms.NumericUpDown();
        this.lblMinQrSize = new System.Windows.Forms.Label();
        this.numMinQrSize = new System.Windows.Forms.NumericUpDown();
        this.lblDebounce = new System.Windows.Forms.Label();
        this.numDebounce = new System.Windows.Forms.NumericUpDown();
        this.chkConfirmSensitive = new System.Windows.Forms.CheckBox();
        
        this.btnSave = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        
        ((System.ComponentModel.ISupportInitialize)(this.numAutoDismiss)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numMaxLength)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numThrottle)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numFpsCap)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numMinQrSize)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numDebounce)).BeginInit();
        this.tabControl.SuspendLayout();
        this.tabClipboardToQr.SuspendLayout();
        this.tabQrToClipboard.SuspendLayout();
        this.SuspendLayout();
        
        // tabControl
        this.tabControl.Controls.Add(this.tabClipboardToQr);
        this.tabControl.Controls.Add(this.tabQrToClipboard);
        this.tabControl.Location = new System.Drawing.Point(12, 12);
        this.tabControl.Name = "tabControl";
        this.tabControl.SelectedIndex = 0;
        this.tabControl.Size = new System.Drawing.Size(460, 280);
        
        // tabClipboardToQr
        this.tabClipboardToQr.Controls.Add(this.lblAutoDismiss);
        this.tabClipboardToQr.Controls.Add(this.numAutoDismiss);
        this.tabClipboardToQr.Controls.Add(this.lblMaxLength);
        this.tabClipboardToQr.Controls.Add(this.numMaxLength);
        this.tabClipboardToQr.Controls.Add(this.lblThrottle);
        this.tabClipboardToQr.Controls.Add(this.numThrottle);
        this.tabClipboardToQr.Controls.Add(this.lblErrorCorrection);
        this.tabClipboardToQr.Controls.Add(this.cmbErrorCorrection);
        this.tabClipboardToQr.Location = new System.Drawing.Point(4, 24);
        this.tabClipboardToQr.Name = "tabClipboardToQr";
        this.tabClipboardToQr.Padding = new System.Windows.Forms.Padding(3);
        this.tabClipboardToQr.Size = new System.Drawing.Size(452, 252);
        this.tabClipboardToQr.Text = "Clipboard → QR";
        this.tabClipboardToQr.UseVisualStyleBackColor = true;
        
        // lblAutoDismiss
        this.lblAutoDismiss.AutoSize = true;
        this.lblAutoDismiss.Location = new System.Drawing.Point(20, 20);
        this.lblAutoDismiss.Name = "lblAutoDismiss";
        this.lblAutoDismiss.Size = new System.Drawing.Size(150, 15);
        this.lblAutoDismiss.Text = "Auto-dismiss (seconds):";
        
        // numAutoDismiss
        this.numAutoDismiss.Location = new System.Drawing.Point(250, 18);
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
        this.numMaxLength.Location = new System.Drawing.Point(250, 53);
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
        this.numThrottle.Location = new System.Drawing.Point(250, 88);
        this.numThrottle.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        this.numThrottle.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
        this.numThrottle.Increment = new decimal(new int[] { 100, 0, 0, 0 });
        this.numThrottle.Name = "numThrottle";
        this.numThrottle.Size = new System.Drawing.Size(100, 23);
        this.numThrottle.Value = new decimal(new int[] { 500, 0, 0, 0 });
        
        // lblErrorCorrection
        this.lblErrorCorrection.AutoSize = true;
        this.lblErrorCorrection.Location = new System.Drawing.Point(20, 125);
        this.lblErrorCorrection.Name = "lblErrorCorrection";
        this.lblErrorCorrection.Size = new System.Drawing.Size(150, 15);
        this.lblErrorCorrection.Text = "Error Correction Level:";
        
        // cmbErrorCorrection
        this.cmbErrorCorrection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbErrorCorrection.FormattingEnabled = true;
        this.cmbErrorCorrection.Items.AddRange(new object[] { "L (Low)", "M (Medium)", "Q (Quartile)", "H (High)" });
        this.cmbErrorCorrection.Location = new System.Drawing.Point(250, 123);
        this.cmbErrorCorrection.Name = "cmbErrorCorrection";
        this.cmbErrorCorrection.Size = new System.Drawing.Size(150, 23);
        
        // tabQrToClipboard
        this.tabQrToClipboard.Controls.Add(this.chkAutoScan);
        this.tabQrToClipboard.Controls.Add(this.lblAutoScanScope);
        this.tabQrToClipboard.Controls.Add(this.cmbAutoScanScope);
        this.tabQrToClipboard.Controls.Add(this.lblFpsCap);
        this.tabQrToClipboard.Controls.Add(this.numFpsCap);
        this.tabQrToClipboard.Controls.Add(this.lblMinQrSize);
        this.tabQrToClipboard.Controls.Add(this.numMinQrSize);
        this.tabQrToClipboard.Controls.Add(this.lblDebounce);
        this.tabQrToClipboard.Controls.Add(this.numDebounce);
        this.tabQrToClipboard.Controls.Add(this.chkConfirmSensitive);
        this.tabQrToClipboard.Location = new System.Drawing.Point(4, 24);
        this.tabQrToClipboard.Name = "tabQrToClipboard";
        this.tabQrToClipboard.Padding = new System.Windows.Forms.Padding(3);
        this.tabQrToClipboard.Size = new System.Drawing.Size(452, 252);
        this.tabQrToClipboard.Text = "QR → Clipboard";
        this.tabQrToClipboard.UseVisualStyleBackColor = true;
        
        // chkAutoScan
        this.chkAutoScan.AutoSize = true;
        this.chkAutoScan.Location = new System.Drawing.Point(20, 20);
        this.chkAutoScan.Name = "chkAutoScan";
        this.chkAutoScan.Size = new System.Drawing.Size(200, 19);
        this.chkAutoScan.Text = "Enable Auto Scan (Default: ON)";
        this.chkAutoScan.UseVisualStyleBackColor = true;
        
        // lblAutoScanScope
        this.lblAutoScanScope.AutoSize = true;
        this.lblAutoScanScope.Location = new System.Drawing.Point(20, 50);
        this.lblAutoScanScope.Name = "lblAutoScanScope";
        this.lblAutoScanScope.Size = new System.Drawing.Size(110, 15);
        this.lblAutoScanScope.Text = "Auto Scan Scope:";
        
        // cmbAutoScanScope
        this.cmbAutoScanScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbAutoScanScope.FormattingEnabled = true;
        this.cmbAutoScanScope.Items.AddRange(new object[] { "Active Window", "Active Monitor", "All Monitors" });
        this.cmbAutoScanScope.Location = new System.Drawing.Point(250, 48);
        this.cmbAutoScanScope.Name = "cmbAutoScanScope";
        this.cmbAutoScanScope.Size = new System.Drawing.Size(150, 23);
        
        // lblFpsCap
        this.lblFpsCap.AutoSize = true;
        this.lblFpsCap.Location = new System.Drawing.Point(20, 85);
        this.lblFpsCap.Name = "lblFpsCap";
        this.lblFpsCap.Size = new System.Drawing.Size(100, 15);
        this.lblFpsCap.Text = "FPS Cap (2-10):";
        
        // numFpsCap
        this.numFpsCap.Location = new System.Drawing.Point(250, 83);
        this.numFpsCap.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
        this.numFpsCap.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        this.numFpsCap.Name = "numFpsCap";
        this.numFpsCap.Size = new System.Drawing.Size(100, 23);
        this.numFpsCap.Value = new decimal(new int[] { 3, 0, 0, 0 });
        
        // lblMinQrSize
        this.lblMinQrSize.AutoSize = true;
        this.lblMinQrSize.Location = new System.Drawing.Point(20, 120);
        this.lblMinQrSize.Name = "lblMinQrSize";
        this.lblMinQrSize.Size = new System.Drawing.Size(140, 15);
        this.lblMinQrSize.Text = "Min QR Size (pixels):";
        
        // numMinQrSize
        this.numMinQrSize.Location = new System.Drawing.Point(250, 118);
        this.numMinQrSize.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
        this.numMinQrSize.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
        this.numMinQrSize.Increment = new decimal(new int[] { 10, 0, 0, 0 });
        this.numMinQrSize.Name = "numMinQrSize";
        this.numMinQrSize.Size = new System.Drawing.Size(100, 23);
        this.numMinQrSize.Value = new decimal(new int[] { 50, 0, 0, 0 });
        
        // lblDebounce
        this.lblDebounce.AutoSize = true;
        this.lblDebounce.Location = new System.Drawing.Point(20, 155);
        this.lblDebounce.Name = "lblDebounce";
        this.lblDebounce.Size = new System.Drawing.Size(150, 15);
        this.lblDebounce.Text = "Debounce (seconds):";
        
        // numDebounce
        this.numDebounce.Location = new System.Drawing.Point(250, 153);
        this.numDebounce.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        this.numDebounce.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
        this.numDebounce.Name = "numDebounce";
        this.numDebounce.Size = new System.Drawing.Size(100, 23);
        this.numDebounce.Value = new decimal(new int[] { 2, 0, 0, 0 });
        
        // chkConfirmSensitive
        this.chkConfirmSensitive.AutoSize = true;
        this.chkConfirmSensitive.Location = new System.Drawing.Point(20, 190);
        this.chkConfirmSensitive.Name = "chkConfirmSensitive";
        this.chkConfirmSensitive.Size = new System.Drawing.Size(230, 19);
        this.chkConfirmSensitive.Text = "Confirm before copying sensitive data";
        this.chkConfirmSensitive.UseVisualStyleBackColor = true;
        
        // btnSave
        this.btnSave.Location = new System.Drawing.Point(280, 305);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(90, 30);
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
        
        // btnCancel
        this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.btnCancel.Location = new System.Drawing.Point(380, 305);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(90, 30);
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        
        // SettingsForm
        this.AcceptButton = this.btnSave;
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(484, 350);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.tabControl);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "SettingsForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "QR Copy-Paste Settings";
        
        ((System.ComponentModel.ISupportInitialize)(this.numAutoDismiss)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numMaxLength)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numThrottle)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numFpsCap)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numMinQrSize)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numDebounce)).EndInit();
        this.tabControl.ResumeLayout(false);
        this.tabClipboardToQr.ResumeLayout(false);
        this.tabClipboardToQr.PerformLayout();
        this.tabQrToClipboard.ResumeLayout(false);
        this.tabQrToClipboard.PerformLayout();
        this.ResumeLayout(false);
    }
}

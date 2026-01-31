namespace QrCopyPaste;

public partial class SettingsForm : Form
{
    public AppSettings Settings { get; private set; }

    public SettingsForm(AppSettings settings)
    {
        // Work with a copy of the provided settings to avoid modifying the original unless saved
        Settings = new AppSettings
        {
            AutoDismissSeconds = settings.AutoDismissSeconds,
            MaxTextLength = settings.MaxTextLength,
            ThrottleMilliseconds = settings.ThrottleMilliseconds,
            IsPaused = settings.IsPaused,
            Mode = settings.Mode,
            ErrorCorrectionLevel = settings.ErrorCorrectionLevel
        };
        InitializeComponent();

        // Load current settings
        numAutoDismiss.Value = Settings.AutoDismissSeconds;
        numMaxLength.Value = Settings.MaxTextLength;
        numThrottle.Value = Settings.ThrottleMilliseconds;
        
        // Map error correction level to combo box index
        cmbErrorCorrection.SelectedIndex = Settings.ErrorCorrectionLevel switch
        {
            QrErrorCorrectionLevel.Low => 0,
            QrErrorCorrectionLevel.Medium => 1,
            QrErrorCorrectionLevel.Quartile => 2,
            QrErrorCorrectionLevel.High => 3,
            _ => 0
        };
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        Settings.AutoDismissSeconds = (int)numAutoDismiss.Value;
        Settings.MaxTextLength = (int)numMaxLength.Value;
        Settings.ThrottleMilliseconds = (int)numThrottle.Value;
        
        // Map combo box index to error correction level
        Settings.ErrorCorrectionLevel = cmbErrorCorrection.SelectedIndex switch
        {
            0 => QrErrorCorrectionLevel.Low,
            1 => QrErrorCorrectionLevel.Medium,
            2 => QrErrorCorrectionLevel.Quartile,
            3 => QrErrorCorrectionLevel.High,
            _ => QrErrorCorrectionLevel.Low
        };
        
        Settings.Save();
        DialogResult = DialogResult.OK;
        Close();
    }
}

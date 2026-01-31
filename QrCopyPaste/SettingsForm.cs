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
        cmbErrorCorrection.SelectedIndex = (int)Settings.ErrorCorrectionLevel;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        Settings.AutoDismissSeconds = (int)numAutoDismiss.Value;
        Settings.MaxTextLength = (int)numMaxLength.Value;
        Settings.ThrottleMilliseconds = (int)numThrottle.Value;
        Settings.ErrorCorrectionLevel = (QrErrorCorrectionLevel)cmbErrorCorrection.SelectedIndex;
        
        Settings.Save();
        DialogResult = DialogResult.OK;
        Close();
    }
}

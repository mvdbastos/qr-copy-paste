using QRCoder;

namespace QrCopyPaste;

public partial class SettingsForm : Form
{
    public AppSettings Settings { get; private set; }

    public SettingsForm(AppSettings settings)
    {
        // Work with a copy of the provided settings to avoid modifying the original unless saved
        Settings = new AppSettings
        {
            CurrentMode = settings.CurrentMode,
            LastMode = settings.LastMode,
            AutoDismissSeconds = settings.AutoDismissSeconds,
            MaxTextLength = settings.MaxTextLength,
            ThrottleMilliseconds = settings.ThrottleMilliseconds,
            IsPaused = settings.IsPaused,
            ErrorCorrectionLevel = settings.ErrorCorrectionLevel,
            AutoScanEnabled = settings.AutoScanEnabled,
            AutoScanScope = settings.AutoScanScope,
            AutoScanFpsCap = settings.AutoScanFpsCap,
            MinQrSize = settings.MinQrSize,
            DebounceSeconds = settings.DebounceSeconds,
            ConfirmSensitivePatterns = settings.ConfirmSensitivePatterns,
            BlockedWindowTitles = new List<string>(settings.BlockedWindowTitles)
        };
        InitializeComponent();

        // Load current settings
        numAutoDismiss.Value = Settings.AutoDismissSeconds;
        numMaxLength.Value = Settings.MaxTextLength;
        numThrottle.Value = Settings.ThrottleMilliseconds;
        
        // Error correction level
        cmbErrorCorrection.SelectedIndex = (int)Settings.ErrorCorrectionLevel;
        
        // Auto-scan settings
        chkAutoScan.Checked = Settings.AutoScanEnabled;
        cmbAutoScanScope.SelectedIndex = (int)Settings.AutoScanScope;
        numFpsCap.Value = Settings.AutoScanFpsCap;
        numMinQrSize.Value = Settings.MinQrSize;
        numDebounce.Value = Settings.DebounceSeconds;
        chkConfirmSensitive.Checked = Settings.ConfirmSensitivePatterns;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        Settings.AutoDismissSeconds = (int)numAutoDismiss.Value;
        Settings.MaxTextLength = (int)numMaxLength.Value;
        Settings.ThrottleMilliseconds = (int)numThrottle.Value;
        Settings.ErrorCorrectionLevel = (QRCodeGenerator.ECCLevel)cmbErrorCorrection.SelectedIndex;
        
        Settings.AutoScanEnabled = chkAutoScan.Checked;
        Settings.AutoScanScope = (AutoScanScope)cmbAutoScanScope.SelectedIndex;
        Settings.AutoScanFpsCap = (int)numFpsCap.Value;
        Settings.MinQrSize = (int)numMinQrSize.Value;
        Settings.DebounceSeconds = (int)numDebounce.Value;
        Settings.ConfirmSensitivePatterns = chkConfirmSensitive.Checked;
        
        Settings.Save();
        DialogResult = DialogResult.OK;
        Close();
    }
}

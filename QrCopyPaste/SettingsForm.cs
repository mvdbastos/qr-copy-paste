namespace QrCopyPaste;

public partial class SettingsForm : Form
{
    public AppSettings Settings { get; private set; }

    public SettingsForm(AppSettings settings)
    {
        Settings = settings;
        InitializeComponent();

        // Load current settings
        numAutoDismiss.Value = settings.AutoDismissSeconds;
        numMaxLength.Value = settings.MaxTextLength;
        numThrottle.Value = settings.ThrottleMilliseconds;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        Settings.AutoDismissSeconds = (int)numAutoDismiss.Value;
        Settings.MaxTextLength = (int)numMaxLength.Value;
        Settings.ThrottleMilliseconds = (int)numThrottle.Value;
        
        Settings.Save();
        DialogResult = DialogResult.OK;
        Close();
    }
}

using Newtonsoft.Json;

namespace QrCopyPaste;

public class AppSettings
{
    // General settings
    public OperationMode CurrentMode { get; set; } = OperationMode.ClipboardToQr;
    public OperationMode LastMode { get; set; } = OperationMode.ClipboardToQr;

    // Clipboard → QR settings
    public int AutoDismissSeconds { get; set; } = 5;
    public int MaxTextLength { get; set; } = 500;
    public bool IsPaused { get; set; } = false;
    public int ThrottleMilliseconds { get; set; } = 500;
    public QRCoder.QRCodeGenerator.ECCLevel ErrorCorrectionLevel { get; set; } = QRCoder.QRCodeGenerator.ECCLevel.L;

    // QR → Clipboard settings
    public bool AutoScanEnabled { get; set; } = true;
    public AutoScanScope AutoScanScope { get; set; } = AutoScanScope.ActiveWindow;
    public int AutoScanFpsCap { get; set; } = 3;
    public int MinQrSize { get; set; } = 50;
    public int DebounceSeconds { get; set; } = 2;
    public bool ConfirmSensitivePatterns { get; set; } = true;
    public List<string> BlockedWindowTitles { get; set; } = new List<string>();

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "QrCopyPaste",
        "settings.json"
    );

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch
        {
            // Intentionally silent - if settings cannot be loaded, use defaults
            // This handles corrupted files, permission issues, etc.
        }
        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsPath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            // Intentionally silent - application continues to work with in-memory settings
            // if persistence fails due to permissions or disk issues
        }
    }
}

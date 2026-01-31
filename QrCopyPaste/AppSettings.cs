using Newtonsoft.Json;

namespace QrCopyPaste;

public enum OperationMode
{
    ClipboardToQr,
    QrToClipboard
}

public enum QrErrorCorrectionLevel
{
    Low,
    Medium,
    Quartile,
    High
}

public class AppSettings
{
    public int AutoDismissSeconds { get; set; } = 5;
    public int MaxTextLength { get; set; } = 500;
    public bool IsPaused { get; set; } = false;
    public int ThrottleMilliseconds { get; set; } = 500;
    public OperationMode Mode { get; set; } = OperationMode.ClipboardToQr;
    public QrErrorCorrectionLevel ErrorCorrectionLevel { get; set; } = QrErrorCorrectionLevel.Low;

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

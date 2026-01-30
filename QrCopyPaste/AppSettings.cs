using Newtonsoft.Json;

namespace QrCopyPaste;

public class AppSettings
{
    public int AutoDismissSeconds { get; set; } = 5;
    public int MaxTextLength { get; set; } = 500;
    public bool IsPaused { get; set; } = false;
    public int ThrottleMilliseconds { get; set; } = 500;

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
            // If loading fails, return default settings
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
            // Silently fail if we can't save settings
        }
    }
}

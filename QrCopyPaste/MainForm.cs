using System.Runtime.InteropServices;

namespace QrCopyPaste;

public partial class MainForm : Form
{
    private const int WM_CLIPBOARDUPDATE = 0x031D;
    private const int WM_HOTKEY = 0x0312;
    private const int HOTKEY_ID = 1;
    private const uint MOD_CONTROL = 0x0004;
    private const uint MOD_SHIFT = 0x0008;
    private const uint VK_Q = 0x51;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool AddClipboardFormatListener(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

    private NotifyIcon? trayIcon;
    private AppSettings settings;
    private string? lastClipboardText;
    private DateTime lastClipboardTime = DateTime.MinValue;
    private QrOverlay? currentOverlay;
    private IntPtr trayIconHandle;

    public MainForm()
    {
        InitializeComponent();
        settings = AppSettings.Load();

        // Hide the form (tray-only app)
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        Opacity = 0;

        // Setup tray icon
        SetupTrayIcon();

        // Register clipboard listener
        AddClipboardFormatListener(Handle);

        // Register global hotkey (Ctrl+Shift+Q)
        RegisterGlobalHotKey();
    }

    private void SetupTrayIcon()
    {
        trayIcon = new NotifyIcon
        {
            Text = "QR Copy-Paste",
            Visible = true,
            ContextMenuStrip = CreateContextMenu()
        };

        // Create a simple icon programmatically
        var bitmap = new Bitmap(16, 16);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            g.FillRectangle(Brushes.Black, 2, 2, 12, 12);
            g.FillRectangle(Brushes.White, 4, 4, 8, 8);
            g.FillRectangle(Brushes.Black, 6, 6, 4, 4);
        }
        
        trayIconHandle = bitmap.GetHicon();
        trayIcon.Icon = Icon.FromHandle(trayIconHandle);
        bitmap.Dispose();
        
        trayIcon.DoubleClick += (s, e) => ShowLastQr();
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();

        var showLastItem = new ToolStripMenuItem("Show Last");
        showLastItem.Click += (s, e) => ShowLastQr();
        menu.Items.Add(showLastItem);

        var pauseResumeItem = new ToolStripMenuItem(settings.IsPaused ? "Resume (Ctrl+Shift+Q)" : "Pause (Ctrl+Shift+Q)");
        pauseResumeItem.Click += (s, e) => TogglePause();
        pauseResumeItem.Tag = "pauseResume";
        menu.Items.Add(pauseResumeItem);

        menu.Items.Add(new ToolStripSeparator());

        var settingsItem = new ToolStripMenuItem("Settings...");
        settingsItem.Click += (s, e) => ShowSettings();
        menu.Items.Add(settingsItem);

        menu.Items.Add(new ToolStripSeparator());

        var exitItem = new ToolStripMenuItem("Exit");
        exitItem.Click += (s, e) => ExitApplication();
        menu.Items.Add(exitItem);

        return menu;
    }

    private void UpdatePauseResumeMenuItem()
    {
        if (trayIcon?.ContextMenuStrip != null)
        {
            foreach (ToolStripItem item in trayIcon.ContextMenuStrip.Items)
            {
                if (item is ToolStripMenuItem menuItem && menuItem.Tag?.ToString() == "pauseResume")
                {
                    menuItem.Text = settings.IsPaused ? "Resume (Ctrl+Shift+Q)" : "Pause (Ctrl+Shift+Q)";
                    break;
                }
            }
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_CLIPBOARDUPDATE)
        {
            HandleClipboardUpdate();
        }
        else if (m.Msg == WM_HOTKEY)
        {
            if (m.WParam.ToInt32() == HOTKEY_ID)
            {
                TogglePause();
            }
        }
        base.WndProc(ref m);
    }

    private void HandleClipboardUpdate()
    {
        if (settings.IsPaused)
            return;

        try
        {
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();

                // Throttle duplicates
                if (text == lastClipboardText && 
                    lastClipboardTime != DateTime.MinValue &&
                    (DateTime.Now - lastClipboardTime).TotalMilliseconds < settings.ThrottleMilliseconds)
                {
                    return;
                }

                // Validate text length
                if (text.Length > settings.MaxTextLength)
                {
                    trayIcon?.ShowBalloonTip(3000, "QR Copy-Paste", 
                        $"Text too long ({text.Length} characters). Max: {settings.MaxTextLength}", 
                        ToolTipIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }

                lastClipboardText = text;
                lastClipboardTime = DateTime.Now;

                // Close existing overlay if any
                currentOverlay?.Close();
                currentOverlay?.Dispose();

                // Show new QR overlay
                currentOverlay = new QrOverlay(text, settings.AutoDismissSeconds);
                currentOverlay.Show();
            }
        }
        catch (Exception ex)
        {
            trayIcon?.ShowBalloonTip(3000, "QR Copy-Paste Error", 
                $"Failed to process clipboard: {ex.Message}", 
                ToolTipIcon.Error);
        }
    }

    private void ShowLastQr()
    {
        if (!string.IsNullOrEmpty(lastClipboardText))
        {
            currentOverlay?.Close();
            currentOverlay?.Dispose();
            currentOverlay = new QrOverlay(lastClipboardText, settings.AutoDismissSeconds);
            currentOverlay.Show();
        }
        else
        {
            trayIcon?.ShowBalloonTip(2000, "QR Copy-Paste", 
                "No text copied yet.", 
                ToolTipIcon.Info);
        }
    }

    private void TogglePause()
    {
        settings.IsPaused = !settings.IsPaused;
        settings.Save();
        UpdatePauseResumeMenuItem();
        
        trayIcon?.ShowBalloonTip(2000, "QR Copy-Paste", 
            settings.IsPaused ? "Monitoring paused" : "Monitoring resumed", 
            ToolTipIcon.Info);
    }

    private void ShowSettings()
    {
        using var settingsForm = new SettingsForm(settings);
        if (settingsForm.ShowDialog() == DialogResult.OK)
        {
            settings = settingsForm.Settings;
        }
    }

    private void ExitApplication()
    {
        RemoveClipboardFormatListener(Handle);
        UnregisterGlobalHotKey();
        trayIcon?.Dispose();
        Application.Exit();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            RemoveClipboardFormatListener(Handle);
            UnregisterGlobalHotKey();
            trayIcon?.Dispose();
            currentOverlay?.Dispose();
            if (trayIconHandle != IntPtr.Zero)
            {
                DestroyIcon(trayIconHandle);
            }
        }
        base.Dispose(disposing);
    }

    private void RegisterGlobalHotKey()
    {
        // Ctrl+Shift+Q
        if (!RegisterHotKey(Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_Q))
        {
            // Hotkey registration failed - another app may have registered it
            // Continue without hotkey - user can still use tray menu
        }
    }

    private void UnregisterGlobalHotKey()
    {
        UnregisterHotKey(Handle, HOTKEY_ID);
    }

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr hIcon);
}

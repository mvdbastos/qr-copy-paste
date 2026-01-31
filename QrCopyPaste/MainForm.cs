using System.Runtime.InteropServices;
using System.Diagnostics;

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
    private ContextMenuStrip? trayContextMenu;
    private AppSettings settings;
    
    // Clipboard → QR mode fields
    private string? lastClipboardText;
    private DateTime lastClipboardTime = DateTime.MinValue;
    private QrOverlay? currentOverlay;
    
    // QR → Clipboard mode fields
    private ScreenCaptureService? captureService;
    private QrDecoderService? decoderService;
    private AutoScanService? autoScanService;
    
    private IntPtr trayIconHandle;

    public MainForm()
    {
        InitializeComponent();
        settings = AppSettings.Load();

        // Restore last mode
        settings.CurrentMode = settings.LastMode;

        // Hide the form (tray-only app)
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
        Opacity = 0;

        // Initialize services
        InitializeServices();

        // Setup tray icon
        SetupTrayIcon();

        // Register clipboard listener (for Clipboard → QR mode)
        AddClipboardFormatListener(Handle);

        // Register global hotkey (Ctrl+Shift+Q for manual scan)
        RegisterGlobalHotKey();

        // Start auto-scan if enabled and in QR → Clipboard mode
        UpdateModeState();
    }

    private void InitializeServices()
    {
        captureService = new ScreenCaptureService();
        decoderService = new QrDecoderService();
        autoScanService = new AutoScanService(settings, captureService, decoderService, OnQrDetected);
    }

    private void OnQrDetected(string decodedText, string? sourceWindow)
    {
        // Ensure we're on UI thread
        if (InvokeRequired)
        {
            Invoke(() => OnQrDetected(decodedText, sourceWindow));
            return;
        }

        // Check if we should confirm sensitive patterns
        if (settings.ConfirmSensitivePatterns && IsSensitivePattern(decodedText))
        {
            ShowQrPreview(decodedText, sourceWindow);
        }
        else
        {
            // Directly copy to clipboard
            CopyToClipboard(decodedText);
            ShowNotification("QR Code Detected", $"Copied to clipboard: {TruncateText(decodedText, 50)}");
        }
    }

    private bool IsSensitivePattern(string text)
    {
        // Check for potential sensitive patterns
        return text.Contains("bitcoin:", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("ethereum:", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("payment", StringComparison.OrdinalIgnoreCase) ||
               text.Contains("wallet", StringComparison.OrdinalIgnoreCase);
    }

    private string TruncateText(string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text;
        return text.Substring(0, maxLength) + "...";
    }

    private void SetupTrayIcon()
    {
        trayContextMenu = CreateContextMenu();
        
        trayIcon = new NotifyIcon
        {
            Text = "QR Copy-Paste",
            Visible = true,
            ContextMenuStrip = trayContextMenu
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

        // Mode selector
        var modeMenu = new ToolStripMenuItem("Mode");
        
        var clipboardToQrItem = new ToolStripMenuItem("Clipboard → QR")
        {
            Checked = settings.CurrentMode == OperationMode.ClipboardToQr,
            Tag = OperationMode.ClipboardToQr
        };
        clipboardToQrItem.Click += (s, e) => SwitchMode(OperationMode.ClipboardToQr);
        modeMenu.DropDownItems.Add(clipboardToQrItem);

        var qrToClipboardItem = new ToolStripMenuItem("QR → Clipboard")
        {
            Checked = settings.CurrentMode == OperationMode.QrToClipboard,
            Tag = OperationMode.QrToClipboard
        };
        qrToClipboardItem.Click += (s, e) => SwitchMode(OperationMode.QrToClipboard);
        modeMenu.DropDownItems.Add(qrToClipboardItem);

        menu.Items.Add(modeMenu);
        menu.Items.Add(new ToolStripSeparator());

        // Mode-specific items
        if (settings.CurrentMode == OperationMode.ClipboardToQr)
        {
            var showLastItem = new ToolStripMenuItem("Show Last QR");
            showLastItem.Click += (s, e) => ShowLastQr();
            menu.Items.Add(showLastItem);

            var pauseResumeItem = new ToolStripMenuItem(settings.IsPaused ? "Resume Monitoring" : "Pause Monitoring");
            pauseResumeItem.Click += (s, e) => TogglePauseClipboardMonitoring();
            pauseResumeItem.Tag = "pauseResume";
            menu.Items.Add(pauseResumeItem);
        }
        else // QR → Clipboard
        {
            var manualScanItem = new ToolStripMenuItem("Manual Scan (Ctrl+Shift+Q)");
            manualScanItem.Click += (s, e) => StartManualScan();
            menu.Items.Add(manualScanItem);

            var autoScanItem = new ToolStripMenuItem(settings.AutoScanEnabled ? "Disable Auto Scan" : "Enable Auto Scan")
            {
                Checked = settings.AutoScanEnabled,
                Tag = "autoScan"
            };
            autoScanItem.Click += (s, e) => ToggleAutoScan();
            menu.Items.Add(autoScanItem);
        }

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


    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_CLIPBOARDUPDATE)
        {
            HandleClipboardUpdate();
        }
        else if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
        {
            // Hotkey behavior depends on mode
            if (settings.CurrentMode == OperationMode.QrToClipboard)
            {
                StartManualScan();
            }
            else
            {
                TogglePauseClipboardMonitoring();
            }
        }
        base.WndProc(ref m);
    }

    private void HandleClipboardUpdate()
    {
        // Only handle in Clipboard → QR mode
        if (settings.CurrentMode != OperationMode.ClipboardToQr)
            return;

        if (settings.IsPaused)
            return;

        try
        {
            // Only process if clipboard contains text (not images, files, etc.)
            string? text = null;

            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                // Prefer Unicode text if available
                text = Clipboard.GetText(TextDataFormat.UnicodeText);
            }
            else if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                // Fallback to plain text
                text = Clipboard.GetText(TextDataFormat.Text);
            }

            if (text == null)
            {
                return;
            }

            // Check for whitespace before length validation to avoid misleading error messages
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

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

            lastClipboardText = text;
            lastClipboardTime = DateTime.Now;

            // Close existing overlay if any
            currentOverlay?.Close();
            currentOverlay?.Dispose();

            // Show new QR overlay
            currentOverlay = new QrOverlay(text, settings.AutoDismissSeconds, settings.ErrorCorrectionLevel);
            currentOverlay.Show();
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
        if (string.IsNullOrEmpty(lastClipboardText))
        {
            trayIcon?.ShowBalloonTip(2000, "QR Copy-Paste", 
                "No text has been copied yet.", 
                ToolTipIcon.Info);
            return;
        }

        currentOverlay?.Close();
        currentOverlay?.Dispose();
        currentOverlay = new QrOverlay(lastClipboardText, settings.AutoDismissSeconds, settings.ErrorCorrectionLevel);
        currentOverlay.Show();
    }

    private void TogglePauseClipboardMonitoring()
    {
        settings.IsPaused = !settings.IsPaused;
        settings.Save();
        UpdateContextMenu();
        
        ShowNotification("QR Copy-Paste", 
            settings.IsPaused ? "Clipboard monitoring paused" : "Clipboard monitoring resumed");
    }

    private void SwitchMode(OperationMode newMode)
    {
        if (settings.CurrentMode == newMode)
            return;

        settings.CurrentMode = newMode;
        settings.LastMode = newMode;
        settings.Save();

        UpdateModeState();
        UpdateContextMenu();
        
        var modeName = newMode == OperationMode.ClipboardToQr ? "Clipboard → QR" : "QR → Clipboard";
        ShowNotification("Mode Changed", $"Switched to {modeName} mode");
    }

    private void UpdateModeState()
    {
        if (settings.CurrentMode == OperationMode.QrToClipboard)
        {
            // Start auto-scan if enabled
            if (settings.AutoScanEnabled)
            {
                autoScanService?.Start();
            }
            else
            {
                autoScanService?.Stop();
            }
        }
        else
        {
            // Stop auto-scan when in Clipboard → QR mode
            autoScanService?.Stop();
        }
    }

    private void UpdateContextMenu()
    {
        trayContextMenu?.Dispose();
        trayContextMenu = CreateContextMenu();
        if (trayIcon != null)
        {
            trayIcon.ContextMenuStrip = trayContextMenu;
        }
    }

    private void ToggleAutoScan()
    {
        settings.AutoScanEnabled = !settings.AutoScanEnabled;
        settings.Save();
        
        if (settings.AutoScanEnabled)
        {
            autoScanService?.Start();
            ShowNotification("Auto Scan", "Auto scan enabled");
        }
        else
        {
            autoScanService?.Stop();
            ShowNotification("Auto Scan", "Auto scan disabled");
        }
        
        UpdateContextMenu();
    }

    private void StartManualScan()
    {
        try
        {
            using var overlay = new RegionSelectionOverlay();
            if (overlay.ShowDialog() == DialogResult.OK)
            {
                var region = overlay.SelectedRegion;
                
                // Capture the selected region
                if (captureService == null || decoderService == null)
                    return;

                using var capture = captureService.CaptureRegion(region);
                var decodedText = decoderService.DecodeQrCode(capture);
                
                if (!string.IsNullOrWhiteSpace(decodedText))
                {
                    ShowQrPreview(decodedText, null);
                }
                else
                {
                    ShowNotification("Manual Scan", "No QR code found in selected region");
                }
            }
        }
        catch (Exception ex)
        {
            ShowNotification("Manual Scan Error", $"Failed to scan region: {ex.Message}");
        }
    }

    private void ShowQrPreview(string decodedText, string? sourceWindow)
    {
        using var previewDialog = new QrPreviewDialog(decodedText, sourceWindow);
        if (previewDialog.ShowDialog() == DialogResult.OK)
        {
            if (previewDialog.ShouldCopyToClipboard)
            {
                CopyToClipboard(decodedText);
                ShowNotification("QR Code", "Copied to clipboard");
            }

            if (previewDialog.ShouldOpenUrl && Uri.TryCreate(decodedText, UriKind.Absolute, out var uri))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = uri.ToString(),
                        UseShellExecute = true
                    });
                }
                catch
                {
                    ShowNotification("Error", "Failed to open URL");
                }
            }

            if (previewDialog.ShouldIgnoreWindow && !string.IsNullOrEmpty(sourceWindow))
            {
                if (!settings.BlockedWindowTitles.Contains(sourceWindow))
                {
                    settings.BlockedWindowTitles.Add(sourceWindow);
                    settings.Save();
                    ShowNotification("Window Blocked", $"Will ignore QR codes from: {sourceWindow}");
                }
            }
        }
    }

    private void CopyToClipboard(string text)
    {
        try
        {
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }
        catch (Exception ex)
        {
            ShowNotification("Clipboard Error", $"Failed to copy: {ex.Message}");
        }
    }

    private void ShowNotification(string title, string message)
    {
        trayIcon?.ShowBalloonTip(3000, title, message, ToolTipIcon.Info);
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
        autoScanService?.Dispose();
        trayIcon?.Dispose();
        Application.Exit();
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

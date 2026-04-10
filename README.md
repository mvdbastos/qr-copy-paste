# QR Copy-Paste

A C# .NET 8 WinForms tray application with dual operation modes for seamless QR code interaction:
1. **Clipboard → QR**: Automatically generate and display QR codes from clipboard text
2. **QR → Clipboard**: Scan QR codes from your screen and inject decoded content into clipboard

## Features

### Clipboard → QR Mode
- **Automatic QR Generation**: Creates QR codes automatically when text is copied to the clipboard
- **Smart Overlay**: Shows QR codes in a borderless, topmost overlay at the bottom-right of the active monitor
- **Copy & Save**: Copy the QR image to clipboard or save as PNG/JPEG/BMP
- **Configurable Error Correction**: Choose between L (Low), M (Medium), Q (Quartile), or H (High) error correction levels
- **Auto-dismiss**: QR codes automatically disappear after a configurable number of seconds
- **Duplicate Throttling**: Prevents showing the same QR code repeatedly within a short time
- **Text Validation**: Ignores non-text clipboard content and text that's too long
- **Pause/Resume**: Toggle clipboard monitoring

### QR → Clipboard Mode
- **Manual Scan (Ctrl+Shift+Q)**: Select a screen region to scan for QR codes
  - Drag to select any area of your screen
  - Instant decoding with preview dialog
  - Actions: Copy to clipboard, Copy & Open URL, Ignore, Always ignore source window
- **Auto Scan (ON by default)**: Automatically detects QR codes on screen
  - Configurable scope: Active window only, Active monitor, or All monitors
  - Adaptive sampling rate (2-10 FPS) for low CPU overhead
  - Smart debouncing prevents duplicate detections
  - Minimum QR size filter to reduce false positives
- **Privacy & Safety**:
  - Opt-out from auto-scan available in settings
  - Confirmation dialog for sensitive patterns (crypto addresses, payment links)
  - Block specific windows from auto-scanning
  - Never scans secure/DRM-protected content
- **URL Support**: Automatically detect URLs and offer "Copy & Open" action

## Dual Operation Modes

The application supports two distinct modes that can be switched from the tray menu:

### Mode: Clipboard → QR
Generate QR codes from text you copy to clipboard. Perfect for sharing text, URLs, or data with mobile devices.

**Tray Menu:**
- Show Last QR
- Pause/Resume Monitoring

### Mode: QR → Clipboard
Scan QR codes from your screen and copy decoded content to clipboard. Perfect for capturing QR codes from websites, videos, or presentations.

**Tray Menu:**
- Manual Scan (Ctrl+Shift+Q)
- Enable/Disable Auto Scan

**Hotkey:**
- **Ctrl+Shift+Q**: 
  - In "Clipboard → QR" mode: Toggles pause/resume monitoring
  - In "QR → Clipboard" mode: Launches manual scan with region selection

## System Tray Menu

- **Mode**: Switch between "Clipboard → QR" and "QR → Clipboard"
- **Mode-specific actions**: Show Last QR / Manual Scan / Auto Scan toggle
- **Settings**: Configure application behavior
- **Exit**: Close the application

## Settings

### Clipboard → QR Settings
- **Auto-dismiss (seconds)**: How long QR codes remain visible (1-60 seconds, default: 5)
- **Max text length (characters)**: Maximum text length for QR generation (50-2000, default: 500)
- **Throttle delay (milliseconds)**: Minimum time between duplicate QR codes (100-5000ms, default: 500)
- **Error Correction Level**: L (Low) / M (Medium) / Q (Quartile) / H (High) - default: L

### QR → Clipboard Settings
- **Enable Auto Scan**: Automatically scan for QR codes on screen (default: ON)
- **Auto Scan Scope**: Active Window / Active Monitor / All Monitors (default: Active Window)
- **FPS Cap**: Scanning frame rate (2-10 FPS, default: 3)
- **Min QR Size**: Minimum QR code size in pixels (20-200, default: 50)
- **Debounce**: Time in seconds before re-detecting same QR (1-10, default: 2)
- **Confirm before copying sensitive data**: Show preview for crypto/payment patterns (default: ON)

## Requirements

- Windows OS
- .NET 8.0 Runtime

## Building

```bash
dotnet build QrCopyPaste.slnx
```

## Running

```bash
dotnet run --project QrCopyPaste/QrCopyPaste.csproj
```

Or run the compiled executable from the bin folder.

## Automated Artifact Releases (GitHub Actions)

This repository includes a release workflow at `.github/workflows/release-artifacts.yml`.

### What It Does

- Builds and publishes the app for `win-x64` using the existing publish profile (`FolderProfile`)
- Packages the publish output as a versioned ZIP file
- Uploads the ZIP as a GitHub Actions artifact (30-day retention)
- Creates or updates a GitHub Release and attaches the same ZIP asset
- Generates GitHub release notes automatically

### Triggers

1. **Tag push**: Push a tag such as `v1.2.3` or `v1.2.3-rc.1`
2. **Manual dispatch**: Run from the Actions tab, optionally providing a `version` input

### Version Rules

- Accepted examples: `1.2.3`, `v1.2.3`, `1.2.3-rc.1`
- Non-tag manual runs must provide the `version` input
- The workflow normalizes release tags to `v<version>`

### Example Release Commands

```bash
git tag v1.0.0
git push origin v1.0.0
```

For prerelease:

```bash
git tag v1.1.0-rc.1
git push origin v1.1.0-rc.1
```

## Usage

### Getting Started
1. Start the application - it will minimize to the system tray
2. Choose your operation mode from the tray icon menu (Mode → Clipboard → QR or QR → Clipboard)
3. The selected mode persists across sessions

### Clipboard → QR Mode
1. Copy any text to the clipboard
2. A QR code will appear at the bottom-right of your active monitor
3. Click "Copy Image" to copy the QR to clipboard
4. Click "Save As..." to export as PNG/JPEG/BMP
5. Click anywhere on the QR to dismiss it immediately
6. Use the tray icon to pause/resume monitoring or show the last QR again

### QR → Clipboard Mode

**Manual Scan:**
1. Press **Ctrl+Shift+Q** or select "Manual Scan" from tray menu
2. Screen will dim with a crosshair cursor
3. Click and drag to select the region containing the QR code
4. Release to scan - a preview dialog will show the decoded content
5. Choose action: Copy, Copy & Open (for URLs), Ignore, or Always Ignore Window

**Auto Scan:**
1. Enable "Auto Scan" from the tray menu (enabled by default)
2. The app continuously scans for QR codes in the configured scope
3. When a QR is detected, decoded content is copied to clipboard (or preview shown for sensitive data)
4. Duplicate detections are prevented within the debounce period
5. Block specific windows from scanning via the "Always Ignore Window" option

## Keyboard Shortcuts

- **Ctrl+Shift+Q**: 
  - Clipboard → QR mode: Pause/Resume clipboard monitoring
  - QR → Clipboard mode: Start manual scan with region selection

## Privacy & Security

- **Auto Scan Opt-Out**: Disable auto-scanning entirely from settings
- **Window Blocking**: Block specific applications from being scanned
- **Sensitive Pattern Detection**: Confirmation required before copying crypto addresses or payment links
- **No Network Access**: All processing happens locally
- **DRM-Protected Content**: Cannot capture secure video streams or DRM-protected windows (by design)

## Technologies Used

- **.NET 8.0**: Modern .NET framework
- **Windows Forms**: Native Windows UI framework
- **QRCoder**: QR code generation library
- **ZXing.Net**: QR code decoding/recognition library
- **Newtonsoft.Json**: JSON serialization for settings persistence

## Known Limitations

- **Windows Only**: Currently supports Windows OS only (uses Windows-specific APIs for screen capture and global hotkeys)
- **DPI Scaling**: Works best with standard DPI settings; very high DPI may require adjustment
- **Screen Capture Restrictions**: Cannot capture from secure/DRM-protected applications
- **QR Detection**: Auto-scan performance depends on QR code size, clarity, and screen content

## Configuration Files

Settings are automatically saved to:
```
%AppData%\QrCopyPaste\settings.json
```

## Troubleshooting

**QR not generating in Clipboard → QR mode:**
- Check if clipboard monitoring is paused (use tray menu to resume)
- Ensure text length is within the max limit (default: 500 characters)
- Verify clipboard contains text (not images or files)

**QR not detected in QR → Clipboard mode:**
- Ensure QR code is clearly visible and not too small
- Try increasing the scan region in manual scan mode
- Reduce "Min QR Size" setting if QR codes are small
- Check that source window is not in the blocked list

**Hotkey not working:**
- Another application may have registered Ctrl+Shift+Q
- Use tray menu alternatives (Manual Scan, Pause/Resume)

**High CPU usage in Auto Scan mode:**
- Reduce FPS Cap setting (default: 3 FPS)
- Change scope to "Active Window" instead of "All Monitors"
- Disable Auto Scan when not needed

## License

See LICENSE file for details.

# QR Copy-Paste

A C# .NET 8 WinForms tray application with dual operation modes for seamless QR code workflows.

## Features

### Dual Operation Modes

#### **Clipboard → QR Mode** (Default)
- **Automatic QR Generation**: Creates QR codes automatically when text is copied to the clipboard
- **Smart Overlay**: Shows QR codes in a borderless, topmost overlay at the bottom-right of the active monitor
- **Copy QR Image**: Copy the QR code image directly to the clipboard
- **Save QR Code**: Export QR codes as PNG images
- **Configurable Error Correction**: Choose from Low, Medium, Quartile, or High error correction levels
- **Auto-dismiss**: QR codes automatically disappear after a configurable number of seconds
- **Duplicate Throttling**: Prevents showing the same QR code repeatedly within a short time
- **Pause/Resume**: Toggle clipboard monitoring with Ctrl+Shift+Q

#### **QR → Clipboard Mode**
- **Manual Scan**: Select a region of your screen containing a QR code (Ctrl+Shift+Q)
- **Screen Capture**: Captures the selected screen region using Windows API
- **QR Decoding**: Decodes QR codes from the captured image using ZXing library
- **Preview Dialog**: Shows decoded text with options to Copy or Ignore
- **Instant Copy**: Decoded text is copied to clipboard with one click

### General Features
- **Single Instance**: Only one instance of the application can run at a time
- **DPI Aware**: Properly handles high DPI displays
- **Text Validation**: Ignores non-text clipboard content and text that's too long
- **Persistent Settings**: Settings are saved in %AppData%\QrCopyPaste\settings.json
- **Mode Persistence**: Remembers your last selected mode across sessions

## System Tray Menu

- **Mode**: Switch between "Clipboard → QR" and "QR → Clipboard" modes
- **Clipboard → QR Mode**:
  - **Show Last**: Display the last generated QR code again
  - **Pause/Resume (Ctrl+Shift+Q)**: Toggle clipboard monitoring
- **QR → Clipboard Mode**:
  - **Manual Scan (Ctrl+Shift+Q)**: Start screen region selection for QR scanning
- **Settings**: Configure application behavior
- **Exit**: Close the application

## Settings

- **Auto-dismiss (seconds)**: How long QR codes remain visible (1-60 seconds, default: 5)
- **Max text length (characters)**: Maximum text length for QR generation (50-2000, default: 500)
- **Throttle delay (milliseconds)**: Minimum time between duplicate QR codes (100-5000ms, default: 500)
- **Error Correction Level**: QR code error correction level - Low (default), Medium, Quartile (Q), or High

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

## Usage

### Clipboard → QR Mode
1. Start the application - it will minimize to the system tray
2. Copy any text to the clipboard
3. A QR code will appear at the bottom-right of your active monitor
4. Click "Copy Image" to copy the QR code to clipboard, or "Save..." to export as PNG
5. The QR code auto-dismisses after the configured time (default: 5 seconds)
6. Click the QR code to dismiss it immediately
7. Use the tray icon to access settings or pause/resume monitoring

### QR → Clipboard Mode
1. Switch to "QR → Clipboard" mode from the tray menu
2. Press Ctrl+Shift+Q or select "Manual Scan" from the tray menu
3. A semi-transparent overlay will appear
4. Click and drag to select the screen region containing the QR code
5. Release the mouse button to capture and decode
6. A preview dialog shows the decoded text
7. Click "Copy to Clipboard" to copy the text, or "Ignore" to cancel
8. Press Escape to cancel the scan at any time

## Technologies Used

- **.NET 8.0**: Modern .NET framework
- **Windows Forms**: Native Windows UI framework
- **QRCoder**: QR code generation library
- **ZXing.Net**: QR code decoding library
- **Newtonsoft.Json**: JSON serialization for settings persistence

## License

See LICENSE file for details.

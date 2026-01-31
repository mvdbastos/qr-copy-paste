# QR Copy-Paste

A C# .NET 8 WinForms tray application that monitors the Windows clipboard and displays QR codes for copied text.

## Features

- **Automatic QR Generation**: Creates QR codes automatically when text is copied to the clipboard
- **Smart Overlay**: Shows QR codes in a borderless, topmost overlay at the bottom-right of the active monitor
- **Single Instance**: Only one instance of the application can run at a time
- **DPI Aware**: Properly handles high DPI displays
- **Auto-dismiss**: QR codes automatically disappear after a configurable number of seconds
- **Duplicate Throttling**: Prevents showing the same QR code repeatedly within a short time
- **Text Validation**: Ignores non-text clipboard content and text that's too long
- **Pause/Resume**: Toggle clipboard monitoring with Ctrl+Shift+Q
- **Configurable Settings**: Customize auto-dismiss time, max text length, and throttle delay
- **Persistent Settings**: Settings are saved in %AppData%\QrCopyPaste\settings.json

## System Tray Menu

- **Show Last**: Display the last generated QR code again
- **Pause/Resume (Ctrl+Shift+Q)**: Toggle clipboard monitoring
- **Settings**: Configure application behavior
- **Exit**: Close the application

## Settings

- **Auto-dismiss (seconds)**: How long QR codes remain visible (1-60 seconds, default: 5)
- **Max text length (characters)**: Maximum text length for QR generation (50-2000, default: 500)
- **Throttle delay (milliseconds)**: Minimum time between duplicate QR codes (100-5000ms, default: 500)

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

1. Start the application - it will minimize to the system tray
2. Copy any text to the clipboard
3. A QR code will appear at the bottom-right of your active monitor
4. The QR code auto-dismisses after the configured time (default: 5 seconds)
5. Click the QR code to dismiss it immediately
6. Use the tray icon to access settings or pause/resume monitoring

## Technologies Used

- **.NET 8.0**: Modern .NET framework
- **Windows Forms**: Native Windows UI framework
- **QRCoder**: QR code generation library
- **Newtonsoft.Json**: JSON serialization for settings persistence

## License

See LICENSE file for details.

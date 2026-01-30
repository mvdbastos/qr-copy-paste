using System.Runtime.InteropServices;

namespace QrCopyPaste;

static class Program
{
    private const string AppGuid = "8F6F0AC4-B9A1-45FD-A8CF-72F04E6BFA9E";
    private static Mutex? appMutex;

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Enable DPI awareness
        SetProcessDPIAware();

        // Ensure single instance
        appMutex = new Mutex(true, AppGuid, out bool createdNew);
        if (!createdNew)
        {
            MessageBox.Show("QR Copy-Paste is already running.", "Already Running", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
        finally
        {
            appMutex?.ReleaseMutex();
            appMutex?.Dispose();
        }
    }    
}
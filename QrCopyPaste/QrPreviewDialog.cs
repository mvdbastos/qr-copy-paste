using System.Drawing;
using System.Windows.Forms;

namespace QrCopyPaste;

/// <summary>
/// Dialog that displays decoded QR content and allows user actions.
/// </summary>
public class QrPreviewDialog : Form
{
    private readonly string decodedText;
    private readonly string? sourceWindow;
    private TextBox? textBox;
    private Button? btnCopy;
    private Button? btnCopyAndOpen;
    private Button? btnIgnore;
    private Button? btnAlwaysIgnore;
    
    public bool ShouldCopyToClipboard { get; private set; }
    public bool ShouldOpenUrl { get; private set; }
    public bool ShouldIgnoreWindow { get; private set; }

    public QrPreviewDialog(string decodedText, string? sourceWindow = null)
    {
        this.decodedText = decodedText;
        this.sourceWindow = sourceWindow;
        
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // Form configuration
        Text = "QR Code Detected";
        Size = new Size(500, 300);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = true;
        TopMost = true;

        // Label
        var label = new Label
        {
            Text = "QR Code Content:",
            Location = new Point(10, 10),
            Size = new Size(480, 20),
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        Controls.Add(label);

        // TextBox for decoded content
        textBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            Location = new Point(10, 35),
            Size = new Size(465, 150),
            Text = decodedText,
            Font = new Font("Consolas", 9)
        };
        Controls.Add(textBox);

        // Buttons
        var buttonY = 195;
        var buttonWidth = 110;
        var buttonHeight = 30;
        var buttonSpacing = 10;

        btnCopy = new Button
        {
            Text = "Copy",
            Location = new Point(10, buttonY),
            Size = new Size(buttonWidth, buttonHeight)
        };
        btnCopy.Click += (s, e) => OnCopy();
        Controls.Add(btnCopy);

        // Check if content is a URL
        if (IsUrl(decodedText))
        {
            btnCopyAndOpen = new Button
            {
                Text = "Copy && Open",
                Location = new Point(10 + buttonWidth + buttonSpacing, buttonY),
                Size = new Size(buttonWidth, buttonHeight)
            };
            btnCopyAndOpen.Click += (s, e) => OnCopyAndOpen();
            Controls.Add(btnCopyAndOpen);
        }

        btnIgnore = new Button
        {
            Text = "Ignore",
            Location = new Point(10 + (buttonWidth + buttonSpacing) * 2, buttonY),
            Size = new Size(buttonWidth, buttonHeight)
        };
        btnIgnore.Click += (s, e) => OnIgnore();
        Controls.Add(btnIgnore);

        if (!string.IsNullOrEmpty(sourceWindow))
        {
            btnAlwaysIgnore = new Button
            {
                Text = "Always Ignore",
                Location = new Point(10 + (buttonWidth + buttonSpacing) * 3, buttonY),
                Size = new Size(buttonWidth, buttonHeight),
                Font = new Font("Segoe UI", 8)
            };
            btnAlwaysIgnore.Click += (s, e) => OnAlwaysIgnore();
            Controls.Add(btnAlwaysIgnore);
        }
    }

    private bool IsUrl(string text)
    {
        return Uri.TryCreate(text, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private void OnCopy()
    {
        ShouldCopyToClipboard = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void OnCopyAndOpen()
    {
        ShouldCopyToClipboard = true;
        ShouldOpenUrl = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void OnIgnore()
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void OnAlwaysIgnore()
    {
        ShouldIgnoreWindow = true;
        DialogResult = DialogResult.Cancel;
        Close();
    }
}

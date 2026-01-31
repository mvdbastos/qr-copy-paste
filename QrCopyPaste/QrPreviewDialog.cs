using System.Drawing;
using System.Windows.Forms;

namespace QrCopyPaste;

public class QrPreviewDialog : Form
{
    private readonly TextBox txtPreview;
    private readonly Button btnCopy;
    private readonly Button btnIgnore;

    public QrPreviewDialog(string decodedText)
    {
        Text = "QR Code Decoded";
        Size = new Size(500, 300);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        // Label
        var lblInfo = new Label
        {
            Text = "Decoded text from QR code:",
            Location = new Point(20, 20),
            AutoSize = true
        };
        Controls.Add(lblInfo);

        // Text preview
        txtPreview = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Location = new Point(20, 50),
            Size = new Size(440, 150),
            Text = decodedText
        };
        Controls.Add(txtPreview);

        // Buttons
        btnCopy = new Button
        {
            Text = "Copy to Clipboard",
            Location = new Point(150, 220),
            Size = new Size(120, 30)
        };
        btnCopy.Click += (s, e) =>
        {
            DialogResult = DialogResult.OK;
            Close();
        };
        Controls.Add(btnCopy);

        btnIgnore = new Button
        {
            Text = "Ignore",
            Location = new Point(280, 220),
            Size = new Size(80, 30)
        };
        btnIgnore.Click += (s, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };
        Controls.Add(btnIgnore);

        AcceptButton = btnCopy;
        CancelButton = btnIgnore;
    }
}

namespace FileOptimizer.UI;

internal sealed class TextPrompt : Form
{
    private readonly TextBox inputTextBox;

    private TextPrompt(string title, string prompt, string initialValue)
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(420, 130);

        var promptLabel = new Label
        {
            AutoSize = true,
            Location = new Point(12, 15),
            Text = prompt
        };

        inputTextBox = new TextBox
        {
            Location = new Point(12, 42),
            Size = new Size(392, 23),
            Text = initialValue
        };

        var okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(248, 85),
            Size = new Size(75, 27)
        };

        var cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(329, 85),
            Size = new Size(75, 27)
        };

        AcceptButton = okButton;
        CancelButton = cancelButton;

        Controls.Add(promptLabel);
        Controls.Add(inputTextBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);
    }

    public static string? Show(IWin32Window owner, string title, string prompt, string initialValue = "")
    {
        using var dialog = new TextPrompt(title, prompt, initialValue);
        return dialog.ShowDialog(owner) == DialogResult.OK
            ? dialog.inputTextBox.Text.Trim()
            : null;
    }
}

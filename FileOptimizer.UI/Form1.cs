using System.Diagnostics;

namespace FileOptimizer.UI;

public partial class Form1 : Form
{
    private List<FileItemView> currentItems = [];
    private ClipboardIntent? clipboardIntent;
    private readonly ContextMenuStrip browserContextMenu = new();
    private readonly ToolStripMenuItem openMenuItem = new("Open");
    private readonly ToolStripMenuItem renameMenuItem = new("Rename");
    private readonly ToolStripMenuItem copyMenuItem = new("Copy");
    private readonly ToolStripMenuItem cutMenuItem = new("Cut");
    private readonly ToolStripMenuItem pasteMenuItem = new("Paste");
    private readonly ToolStripMenuItem deleteMenuItem = new("Delete");
    private readonly ToolStripMenuItem compressMenuItem = new("Compress");
    private readonly ToolStripMenuItem decompressMenuItem = new("Decompress");
    private readonly ToolStripMenuItem newFileMenuItem = new("New File");
    private readonly ToolStripMenuItem newFolderMenuItem = new("New Folder");
    private readonly ToolStripMenuItem refreshMenuItem = new("Refresh");
    private readonly ToolStripMenuItem analysisMenuItem = new("Analysis");

    public Form1()
    {
        InitializeComponent();
        ConfigureUi();
    }

    private void ConfigureUi()
    {
        sortComboBox.DataSource = Enum.GetValues<SortOption>();
        sortComboBox.SelectedItem = SortOption.Name;

        filesGrid.AutoGenerateColumns = false;
        pathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        SetupContextMenu();
        filesGrid.ContextMenuStrip = browserContextMenu;
        filesGrid.MouseDown += filesGrid_MouseDown;
    }

    private void SetupContextMenu()
    {
        openMenuItem.Click += (_, _) =>
        {
            var selected = GetSelectedItem();
            if (selected is not null)
            {
                OpenItem(selected);
            }
        };
        renameMenuItem.Click += (_, _) => RenameSelectedItem();
        copyMenuItem.Click += (_, _) => CopySelectedItem();
        cutMenuItem.Click += (_, _) => CutSelectedItem();
        pasteMenuItem.Click += (_, _) => PasteClipboardIntent();
        deleteMenuItem.Click += (_, _) => deleteButton_Click(this, EventArgs.Empty);
        compressMenuItem.Click += (_, _) => compressButton_Click(this, EventArgs.Empty);
        decompressMenuItem.Click += (_, _) => decompressButton_Click(this, EventArgs.Empty);
        newFileMenuItem.Click += (_, _) => CreateNewFile();
        newFolderMenuItem.Click += (_, _) => CreateNewFolder();
        refreshMenuItem.Click += (_, _) => RefreshDirectory();
        analysisMenuItem.Click += (_, _) => analysisButton_Click(this, EventArgs.Empty);

        browserContextMenu.Items.AddRange(
        [
            openMenuItem,
            renameMenuItem,
            copyMenuItem,
            cutMenuItem,
            pasteMenuItem,
            deleteMenuItem,
            compressMenuItem,
            decompressMenuItem,
            new ToolStripSeparator(),
            newFileMenuItem,
            newFolderMenuItem,
            new ToolStripSeparator(),
            refreshMenuItem,
            analysisMenuItem
        ]);

        browserContextMenu.Opening += browserContextMenu_Opening;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        RefreshDirectory();
    }

    private void browseButton_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            InitialDirectory = Directory.Exists(pathTextBox.Text) ? pathTextBox.Text : string.Empty
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            pathTextBox.Text = dialog.SelectedPath;
            RefreshDirectory();
        }
    }

    private void refreshButton_Click(object sender, EventArgs e) => RefreshDirectory();

    private void upButton_Click(object sender, EventArgs e)
    {
        try
        {
            var currentPath = new DirectoryInfo(pathTextBox.Text);
            if (currentPath.Parent is not null)
            {
                pathTextBox.Text = currentPath.Parent.FullName;
                RefreshDirectory();
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void searchButton_Click(object sender, EventArgs e)
    {
        try
        {
            var path = pathTextBox.Text.Trim();
            var query = searchTextBox.Text.Trim();
            var items = string.IsNullOrWhiteSpace(query)
                ? NativeMethods.GetDirectoryContents(path, GetSortOption())
                : NativeMethods.SearchDirectoryContents(path, query, GetSortOption());

            BindItems(items);
            UpdateStatus($"{items.Count} item(s) shown");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void clearSearchButton_Click(object sender, EventArgs e)
    {
        searchTextBox.Clear();
        RefreshDirectory();
    }

    private void analysisButton_Click(object sender, EventArgs e)
    {
        var selectedFile = filesGrid.CurrentRow?.DataBoundItem is FileItemView item && !item.IsDirectory
            ? item.Path
            : null;

        using var form = new AnalysisForm(pathTextBox.Text, selectedFile);
        form.ShowDialog(this);
    }

    private void filesGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= currentItems.Count)
        {
            return;
        }

        OpenItem(currentItems[e.RowIndex]);
    }

    private void copyButton_Click(object sender, EventArgs e)
    {
        CopySelectedItem();
    }

    private void cutButton_Click(object sender, EventArgs e)
    {
        CutSelectedItem();
    }

    private void pasteButton_Click(object sender, EventArgs e)
    {
        PasteClipboardIntent();
    }

    private void CopySelectedItem()
    {
        var selected = GetSelectedItem();
        if (selected is null)
        {
            return;
        }

        clipboardIntent = new ClipboardIntent(selected.Path, selected.Name, ClipboardAction.Copy);
        UpdateClipboardStatus();
    }

    private void CutSelectedItem()
    {
        var selected = GetSelectedItem();
        if (selected is null)
        {
            return;
        }

        clipboardIntent = new ClipboardIntent(selected.Path, selected.Name, ClipboardAction.Cut);
        UpdateClipboardStatus();
    }

    private void PasteClipboardIntent()
    {
        if (clipboardIntent is null)
        {
            ShowError("Clipboard is empty.");
            return;
        }

        var currentDirectory = GetCurrentDirectoryPath();
        if (currentDirectory is null)
        {
            return;
        }

        var destinationPath = Path.Combine(currentDirectory, clipboardIntent.Name);
        if (string.Equals(destinationPath, clipboardIntent.SourcePath, StringComparison.OrdinalIgnoreCase))
        {
            ShowError("Source and destination are the same.");
            return;
        }

        if (File.Exists(destinationPath) || Directory.Exists(destinationPath))
        {
            ShowError("An item with the same name already exists in this folder.");
            return;
        }

        try
        {
            if (clipboardIntent.Action == ClipboardAction.Copy)
            {
                NativeMethods.CopyPath(clipboardIntent.SourcePath, destinationPath);
                UpdateStatus($"Copied {clipboardIntent.Name}");
            }
            else
            {
                NativeMethods.MovePath(clipboardIntent.SourcePath, destinationPath);
                UpdateStatus($"Moved {clipboardIntent.Name}");
                clipboardIntent = null;
            }

            UpdateClipboardStatus();
            RefreshDirectory();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void deleteButton_Click(object sender, EventArgs e)
    {
        var selected = GetSelectedItem();
        if (selected is null)
        {
            return;
        }

        var result = MessageBox.Show(
            this,
            $"Delete '{selected.Name}'?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result != DialogResult.Yes)
        {
            return;
        }

        try
        {
            NativeMethods.DeletePath(selected.Path);
            UpdateStatus($"Deleted {selected.Name}");
            if (clipboardIntent is not null &&
                string.Equals(clipboardIntent.SourcePath, selected.Path, StringComparison.OrdinalIgnoreCase))
            {
                clipboardIntent = null;
                UpdateClipboardStatus();
            }

            RefreshDirectory();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void compressButton_Click(object sender, EventArgs e)
    {
        var selected = GetSelectedItem();
        if (selected is null)
        {
            return;
        }

        using var dialog = new SaveFileDialog
        {
            InitialDirectory = Path.GetDirectoryName(selected.Path)
        };

        if (selected.IsDirectory)
        {
            dialog.Filter = "ZIP archive (*.zip)|*.zip";
            dialog.DefaultExt = "zip";
            dialog.FileName = $"{selected.Name}.zip";
        }
        else
        {
            dialog.Filter = "Zstandard archive (*.zst)|*.zst|ZIP archive (*.zip)|*.zip";
            dialog.DefaultExt = "zst";
            dialog.FileName = $"{selected.Name}.zst";
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var format = GetCompressionFormatFromPath(dialog.FileName);
            if (selected.IsDirectory && format != CompressionFormat.Zip)
            {
                ShowError("Folders can currently be compressed only as ZIP archives.");
                return;
            }

            NativeMethods.CompressPath(selected.Path, dialog.FileName, format);
            UpdateStatus($"Compressed to {dialog.FileName}");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void decompressButton_Click(object sender, EventArgs e)
    {
        var selected = GetSelectedItem();
        if (selected is null)
        {
            return;
        }

        if (selected.IsDirectory)
        {
            ShowError("Decompression is currently file-only.");
            return;
        }

        var extension = Path.GetExtension(selected.Path);
        if (extension.Equals(".zip", StringComparison.OrdinalIgnoreCase))
        {
            using var folderDialog = new FolderBrowserDialog
            {
                InitialDirectory = Path.GetDirectoryName(selected.Path) ?? string.Empty
            };

            if (folderDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                NativeMethods.DecompressPath(selected.Path, folderDialog.SelectedPath, CompressionFormat.Zip);
                UpdateStatus($"Extracted ZIP to {folderDialog.SelectedPath}");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            return;
        }

        if (!extension.Equals(".zst", StringComparison.OrdinalIgnoreCase))
        {
            ShowError("Only .zst and .zip archives are currently supported.");
            return;
        }

        var outputName = selected.Name.EndsWith(".zst", StringComparison.OrdinalIgnoreCase)
            ? selected.Name[..^4]
            : $"{selected.Name}.out";

        using var dialog = new SaveFileDialog
        {
            FileName = outputName,
            InitialDirectory = Path.GetDirectoryName(selected.Path)
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            NativeMethods.DecompressPath(selected.Path, dialog.FileName, CompressionFormat.Zstd);
            UpdateStatus($"Decompressed to {dialog.FileName}");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void pathTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            RefreshDirectory();
        }
    }

    private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            searchButton.PerformClick();
        }
    }

    private void sortOptionsChanged(object sender, EventArgs e)
    {
        if (Visible)
        {
            RefreshDirectory();
        }
    }

    private void RefreshDirectory()
    {
        try
        {
            var items = NativeMethods.GetDirectoryContents(pathTextBox.Text.Trim(), GetSortOption());
            BindItems(items);
            UpdateStatus($"{items.Count} item(s) loaded");
        }
        catch (DllNotFoundException)
        {
            ShowError("CoreEngine.dll was not found. Build the CoreEngine C++ project first, then run the UI again.");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void BindItems(List<FileItemView> items)
    {
        currentItems = items;
        filesGrid.DataSource = null;
        filesGrid.DataSource = currentItems;
    }

    private NativeSortOptions GetSortOption()
    {
        var selectedSort = sortComboBox.SelectedItem is SortOption value ? value : SortOption.Name;
        return new NativeSortOptions(selectedSort, ascendingCheckBox.Checked, directoriesFirstCheckBox.Checked);
    }

    private FileItemView? GetSelectedItem(bool showError = true)
    {
        if (filesGrid.CurrentRow?.DataBoundItem is not FileItemView item)
        {
            if (showError)
            {
                ShowError("Select an item first.");
            }

            return null;
        }

        return item;
    }

    private void OpenItem(FileItemView selected)
    {
        if (selected.IsDirectory)
        {
            pathTextBox.Text = selected.Path;
            RefreshDirectory();
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = selected.Path,
                UseShellExecute = true
            });

            UpdateStatus($"Opened {selected.Name}");
        }
        catch (Exception ex)
        {
            ShowError($"Could not open '{selected.Name}': {ex.Message}");
        }
    }

    private void RenameSelectedItem()
    {
        var selected = GetSelectedItem();
        if (selected is null)
        {
            return;
        }

        var newName = TextPrompt.Show(this, "Rename", "Enter the new name:", selected.Name);
        if (string.IsNullOrWhiteSpace(newName) || string.Equals(newName, selected.Name, StringComparison.Ordinal))
        {
            return;
        }

        var parent = Path.GetDirectoryName(selected.Path);
        if (string.IsNullOrWhiteSpace(parent))
        {
            ShowError("Could not determine the parent folder for this item.");
            return;
        }

        try
        {
            NativeMethods.RenamePath(selected.Path, Path.Combine(parent, newName));
            UpdateStatus($"Renamed {selected.Name} to {newName}");
            RefreshDirectory();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void CreateNewFile()
    {
        var currentDirectory = GetCurrentDirectoryPath();
        if (currentDirectory is null)
        {
            return;
        }

        var fileName = TextPrompt.Show(this, "New File", "Enter the file name:", "NewFile.txt");
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        try
        {
            NativeMethods.CreateEmptyFile(Path.Combine(currentDirectory, fileName));
            UpdateStatus($"Created file {fileName}");
            RefreshDirectory();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void CreateNewFolder()
    {
        var currentDirectory = GetCurrentDirectoryPath();
        if (currentDirectory is null)
        {
            return;
        }

        var folderName = TextPrompt.Show(this, "New Folder", "Enter the folder name:", "New Folder");
        if (string.IsNullOrWhiteSpace(folderName))
        {
            return;
        }

        try
        {
            NativeMethods.CreateDirectory(Path.Combine(currentDirectory, folderName));
            UpdateStatus($"Created folder {folderName}");
            RefreshDirectory();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private string? GetCurrentDirectoryPath()
    {
        var path = pathTextBox.Text.Trim();
        if (!Directory.Exists(path))
        {
            ShowError("Current path is not a valid folder.");
            return null;
        }

        return path;
    }

    private void filesGrid_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            return;
        }

        var hit = filesGrid.HitTest(e.X, e.Y);
        if (hit.RowIndex >= 0)
        {
            filesGrid.ClearSelection();
            filesGrid.Rows[hit.RowIndex].Selected = true;
            filesGrid.CurrentCell = filesGrid.Rows[hit.RowIndex].Cells[0];
        }
        else
        {
            filesGrid.ClearSelection();
        }
    }

    private void browserContextMenu_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var selected = GetSelectedItem(showError: false);
        var hasSelection = selected is not null;
        var isFile = selected is not null && !selected.IsDirectory;

        openMenuItem.Enabled = hasSelection;
        renameMenuItem.Enabled = hasSelection;
        copyMenuItem.Enabled = hasSelection;
        cutMenuItem.Enabled = hasSelection;
        pasteMenuItem.Enabled = clipboardIntent is not null;
        deleteMenuItem.Enabled = hasSelection;
        compressMenuItem.Enabled = hasSelection;
        decompressMenuItem.Enabled = isFile;
    }

    private void UpdateClipboardStatus()
    {
        if (clipboardIntent is null)
        {
            UpdateStatus("Clipboard cleared");
            return;
        }

        var verb = clipboardIntent.Action == ClipboardAction.Copy ? "Copied" : "Cut";
        UpdateStatus($"{verb} {clipboardIntent.Name}. Choose a destination and paste.");
    }

    private sealed record ClipboardIntent(string SourcePath, string Name, ClipboardAction Action);

    private enum ClipboardAction
    {
        Copy,
        Cut
    }

    private void UpdateStatus(string message) => statusLabel.Text = message;

    private static CompressionFormat GetCompressionFormatFromPath(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".zip" => CompressionFormat.Zip,
            ".zst" => CompressionFormat.Zstd,
            _ => throw new InvalidOperationException("Choose either a .zst or .zip output file.")
        };
    }

    private void ShowError(string message)
    {
        UpdateStatus(message);
        MessageBox.Show(this, message, "File Optimizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

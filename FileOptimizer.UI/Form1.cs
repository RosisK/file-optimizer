using System.Diagnostics;

namespace FileOptimizer.UI;

public partial class Form1 : Form
{
    private List<FileItemView> currentItems = [];

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

    private void filesGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= currentItems.Count)
        {
            return;
        }

        var selected = currentItems[e.RowIndex];
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

    private void copyButton_Click(object sender, EventArgs e)
    {
        var selected = GetSelectedItem();
        if (selected is null)
        {
            return;
        }

        if (selected.IsDirectory)
        {
            ShowError("This starter UI only copies files right now.");
            return;
        }

        using var dialog = new SaveFileDialog
        {
            FileName = selected.Name,
            InitialDirectory = Path.GetDirectoryName(selected.Path)
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            NativeMethods.CopyFile(selected.Path, dialog.FileName);
            UpdateStatus($"Copied to {dialog.FileName}");
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

        if (selected.IsDirectory)
        {
            ShowError("This starter UI only deletes files right now.");
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
            NativeMethods.DeleteFile(selected.Path);
            UpdateStatus($"Deleted {selected.Name}");
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

    private FileItemView? GetSelectedItem()
    {
        if (filesGrid.CurrentRow?.DataBoundItem is not FileItemView item)
        {
            ShowError("Select a file first.");
            return null;
        }

        return item;
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

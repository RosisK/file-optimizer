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
        if (!selected.IsDirectory)
        {
            return;
        }

        pathTextBox.Text = selected.Path;
        RefreshDirectory();
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

        if (selected.IsDirectory)
        {
            ShowError("Compression is currently file-only.");
            return;
        }

        using var dialog = new SaveFileDialog
        {
            FileName = $"{selected.Name}.zst",
            InitialDirectory = Path.GetDirectoryName(selected.Path)
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            NativeMethods.CompressFile(selected.Path, dialog.FileName);
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
            NativeMethods.DecompressFile(selected.Path, dialog.FileName);
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

    private void ShowError(string message)
    {
        UpdateStatus(message);
        MessageBox.Show(this, message, "File Optimizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

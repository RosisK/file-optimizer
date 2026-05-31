using System.Diagnostics;

namespace FileOptimizer.UI;

public partial class Form1 : Form
{
    private List<FileItemView> currentItems = [];
    private ClipboardIntent? clipboardIntent;
    private readonly List<string> navigationHistory = [];
    private int navigationIndex = -1;
    private bool isDraggingSelection;
    private int dragSelectionStartIndex = -1;
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
        KeyPreview = true;
        sortComboBox.DataSource = Enum.GetValues<SortOption>();
        sortComboBox.SelectedItem = SortOption.Name;

        filesGrid.AutoGenerateColumns = false;
        pathTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        SetupContextMenu();
        filesGrid.ContextMenuStrip = browserContextMenu;
        filesGrid.MouseDown += filesGrid_MouseDown;
        filesGrid.MouseMove += filesGrid_MouseMove;
        filesGrid.MouseUp += filesGrid_MouseUp;
        filesGrid.SelectionChanged += filesGrid_SelectionChanged;
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
        NavigateTo(pathTextBox.Text, addToHistory: true);
    }

    private void browseButton_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            InitialDirectory = Directory.Exists(pathTextBox.Text) ? pathTextBox.Text : string.Empty
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            NavigateTo(dialog.SelectedPath, addToHistory: true);
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
                NavigateTo(currentPath.Parent.FullName, addToHistory: true);
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
        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            return;
        }

        clipboardIntent = new ClipboardIntent(
            selectedItems.Select(item => new ClipboardItemIntent(item.Path, item.Name)).ToList(),
            ClipboardAction.Copy);
        LogUi($"Clipboard: copy {selectedItems.Count} item(s).");
        UpdateClipboardStatus();
    }

    private void CutSelectedItem()
    {
        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            return;
        }

        clipboardIntent = new ClipboardIntent(
            selectedItems.Select(item => new ClipboardItemIntent(item.Path, item.Name)).ToList(),
            ClipboardAction.Cut);
        LogUi($"Clipboard: cut {selectedItems.Count} item(s).");
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

        var failures = new List<string>();
        var failedCutItems = new List<ClipboardItemIntent>();
        var completed = 0;
        var action = clipboardIntent.Action;
        try
        {
            foreach (var item in clipboardIntent.Items)
            {
                var destinationPath = Path.Combine(currentDirectory, item.Name);
                if (string.Equals(destinationPath, item.SourcePath, StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add($"{item.Name}: source and destination are the same");
                    if (action == ClipboardAction.Cut)
                    {
                        failedCutItems.Add(item);
                    }

                    continue;
                }

                if (File.Exists(destinationPath) || Directory.Exists(destinationPath))
                {
                    failures.Add($"{item.Name}: destination already exists");
                    if (action == ClipboardAction.Cut)
                    {
                        failedCutItems.Add(item);
                    }

                    continue;
                }

                try
                {
                    if (action == ClipboardAction.Copy)
                    {
                        NativeMethods.CopyPath(item.SourcePath, destinationPath);
                    }
                    else
                    {
                        NativeMethods.MovePath(item.SourcePath, destinationPath);
                    }

                    completed++;
                }
                catch (Exception ex)
                {
                    failures.Add($"{item.Name}: {ex.Message}");
                    if (action == ClipboardAction.Cut)
                    {
                        failedCutItems.Add(item);
                    }
                }
            }

            if (action == ClipboardAction.Cut)
            {
                clipboardIntent = failedCutItems.Count == 0
                    ? null
                    : clipboardIntent with { Items = failedCutItems };
            }

            var verb = action == ClipboardAction.Cut ? "Moved" : "Copied";
            LogUi($"Paste batch -> {completed} completed, {failures.Count} failed.");
            UpdateStatus($"{verb} {completed} item(s)" + (failures.Count > 0 ? $" with {failures.Count} issue(s)" : string.Empty));
            RefreshDirectory();

            if (failures.Count > 0)
            {
                ShowError("Some items could not be pasted:\n\n" + string.Join(Environment.NewLine, failures));
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void deleteButton_Click(object sender, EventArgs e)
    {
        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            return;
        }

        var result = MessageBox.Show(
            this,
            selectedItems.Count == 1
                ? $"Delete '{selectedItems[0].Name}'?"
                : $"Delete {selectedItems.Count} selected item(s)?",
            "Confirm delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result != DialogResult.Yes)
        {
            return;
        }

        try
        {
            var failures = new List<string>();
            var deleted = 0;
            foreach (var item in selectedItems)
            {
                try
                {
                    NativeMethods.DeletePath(item.Path);
                    deleted++;
                }
                catch (Exception ex)
                {
                    failures.Add($"{item.Name}: {ex.Message}");
                }
            }

            LogUi($"Delete batch -> {deleted} completed, {failures.Count} failed.");
            RemoveDeletedItemsFromClipboard(selectedItems);
            UpdateStatus($"Deleted {deleted} item(s)" + (failures.Count > 0 ? $" with {failures.Count} issue(s)" : string.Empty));

            RefreshDirectory();

            if (failures.Count > 0)
            {
                ShowError("Some items could not be deleted:\n\n" + string.Join(Environment.NewLine, failures));
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void compressButton_Click(object sender, EventArgs e)
    {
        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            return;
        }

        if (selectedItems.Count > 1)
        {
            CompressSelectedItemsAsZip(selectedItems);
            return;
        }

        var selected = selectedItems[0];

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

            LogUi($"Compress as {format} -> '{Path.GetFileName(dialog.FileName)}'.");
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
        if (GetSelectedItems(showError: false).Count > 1)
        {
            ShowError("Decompression currently supports one archive at a time.");
            return;
        }

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
                LogUi($"Extract ZIP -> '{folderDialog.SelectedPath}'.");
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
            LogUi($"Decompress Zstd -> '{Path.GetFileName(dialog.FileName)}'.");
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
            NavigateTo(pathTextBox.Text.Trim(), addToHistory: true);
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
        if (string.IsNullOrWhiteSpace(pathTextBox.Text))
        {
            return;
        }

        NavigateTo(pathTextBox.Text.Trim(), addToHistory: false);
    }

    private void NavigateTo(string path, bool addToHistory)
    {
        try
        {
            var normalizedPath = path.Trim();
            var items = NativeMethods.GetDirectoryContents(normalizedPath, GetSortOption());
            pathTextBox.Text = normalizedPath;
            BindItems(items);
            UpdateStatus($"{items.Count} item(s) loaded");

            if (addToHistory)
            {
                RecordNavigation(normalizedPath);
            }
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
        var selectedItems = GetSelectedItems(showError);
        if (selectedItems.Count == 0)
        {
            return null;
        }

        return selectedItems[0];
    }

    private List<FileItemView> GetSelectedItems(bool showError = true)
    {
        var selectedItems = filesGrid.SelectedRows
            .Cast<DataGridViewRow>()
            .OrderBy(row => row.Index)
            .Select(row => row.DataBoundItem)
            .OfType<FileItemView>()
            .ToList();

        if (selectedItems.Count == 0 &&
            filesGrid.CurrentRow?.Selected == true &&
            filesGrid.CurrentRow.DataBoundItem is FileItemView currentItem)
        {
            selectedItems.Add(currentItem);
        }

        if (selectedItems.Count == 0 && showError)
        {
            ShowError("Select one or more items first.");
        }

        return selectedItems;
    }

    private void OpenItem(FileItemView selected)
    {
        if (selected.IsDirectory)
        {
            NavigateTo(selected.Path, addToHistory: true);
            return;
        }

        try
        {
            LogUi($"Open file '{selected.Name}'.");
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

    private void RecordNavigation(string path)
    {
        if (navigationIndex >= 0 &&
            navigationIndex < navigationHistory.Count &&
            string.Equals(navigationHistory[navigationIndex], path, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (navigationIndex < navigationHistory.Count - 1)
        {
            navigationHistory.RemoveRange(navigationIndex + 1, navigationHistory.Count - navigationIndex - 1);
        }

        navigationHistory.Add(path);
        navigationIndex = navigationHistory.Count - 1;
    }

    private void RenameSelectedItem()
    {
        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            return;
        }

        if (selectedItems.Count > 1)
        {
            ShowError("Rename works on one item at a time.");
            return;
        }

        var selected = selectedItems[0];
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
            LogUi($"Rename '{selected.Name}' -> '{newName}'.");
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
            LogUi($"Create file '{fileName}'.");
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
            LogUi($"Create folder '{folderName}'.");
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
        if (e.Button == MouseButtons.Left)
        {
            var leftHit = filesGrid.HitTest(e.X, e.Y);
            isDraggingSelection = leftHit.RowIndex >= 0 &&
                (ModifierKeys & (Keys.Control | Keys.Shift)) == Keys.None;
            dragSelectionStartIndex = isDraggingSelection ? leftHit.RowIndex : -1;
            return;
        }

        if (e.Button != MouseButtons.Right)
        {
            return;
        }

        var hit = filesGrid.HitTest(e.X, e.Y);
        if (hit.RowIndex >= 0)
        {
            if (!filesGrid.Rows[hit.RowIndex].Selected)
            {
                filesGrid.ClearSelection();
                filesGrid.Rows[hit.RowIndex].Selected = true;
            }

            filesGrid.CurrentCell = filesGrid.Rows[hit.RowIndex].Cells[0];
        }
        else
        {
            filesGrid.ClearSelection();
        }
    }

    private void filesGrid_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!isDraggingSelection || dragSelectionStartIndex < 0 || e.Button != MouseButtons.Left)
        {
            return;
        }

        var hit = filesGrid.HitTest(e.X, e.Y);
        if (hit.RowIndex < 0)
        {
            return;
        }

        var start = Math.Min(dragSelectionStartIndex, hit.RowIndex);
        var end = Math.Max(dragSelectionStartIndex, hit.RowIndex);
        filesGrid.ClearSelection();

        for (var index = start; index <= end; index++)
        {
            filesGrid.Rows[index].Selected = true;
        }
    }

    private void filesGrid_MouseUp(object? sender, MouseEventArgs e)
    {
        isDraggingSelection = false;
        dragSelectionStartIndex = -1;
    }

    private void browserContextMenu_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var selectedItems = GetSelectedItems(showError: false);
        var hasSelection = selectedItems.Count > 0;
        var singleSelection = selectedItems.Count == 1;
        var isFile = singleSelection && !selectedItems[0].IsDirectory;

        openMenuItem.Enabled = singleSelection;
        renameMenuItem.Enabled = singleSelection;
        copyMenuItem.Enabled = hasSelection;
        cutMenuItem.Enabled = hasSelection;
        pasteMenuItem.Enabled = clipboardIntent is not null;
        deleteMenuItem.Enabled = hasSelection;
        compressMenuItem.Enabled = hasSelection;
        decompressMenuItem.Enabled = isFile;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Alt | Keys.Left) && GoBack())
        {
            return true;
        }

        if (keyData == (Keys.Alt | Keys.Right) && GoForward())
        {
            return true;
        }

        if (keyData == (Keys.Alt | Keys.Up) && GoUp())
        {
            return true;
        }

        if (keyData == (Keys.Control | Keys.L))
        {
            pathTextBox.Focus();
            pathTextBox.SelectAll();
            return true;
        }

        if (keyData == (Keys.Alt | Keys.D))
        {
            pathTextBox.Focus();
            pathTextBox.SelectAll();
            return true;
        }

        if (keyData == (Keys.Control | Keys.F))
        {
            searchTextBox.Focus();
            searchTextBox.SelectAll();
            return true;
        }

        if (keyData == (Keys.Control | Keys.Shift | Keys.N))
        {
            CreateNewFolder();
            return true;
        }

        if (keyData == (Keys.Control | Keys.N))
        {
            CreateNewFile();
            return true;
        }

        if (keyData == Keys.Escape && IsTextInputFocused())
        {
            FocusFileGrid();
            return true;
        }

        if (IsTextInputFocused())
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        if (keyData == (Keys.Control | Keys.C))
        {
            CopySelectedItem();
            return true;
        }

        if (keyData == (Keys.Control | Keys.X))
        {
            CutSelectedItem();
            return true;
        }

        if (keyData == (Keys.Control | Keys.V))
        {
            PasteClipboardIntent();
            return true;
        }

        if (keyData == (Keys.Control | Keys.A))
        {
            filesGrid.SelectAll();
            UpdateStatus($"{filesGrid.SelectedRows.Count} item(s) selected");
            return true;
        }

        if (keyData == Keys.F5 || keyData == (Keys.Control | Keys.R))
        {
            RefreshDirectory();
            return true;
        }

        if (keyData == Keys.Apps || keyData == (Keys.Shift | Keys.F10))
        {
            ShowContextMenuForKeyboard();
            return true;
        }

        if (keyData == Keys.Delete)
        {
            deleteButton_Click(this, EventArgs.Empty);
            return true;
        }

        if (keyData == Keys.F2)
        {
            RenameSelectedItem();
            return true;
        }

        if (keyData == Keys.Enter)
        {
            var selectedItems = GetSelectedItems(showError: false);
            if (selectedItems.Count == 1)
            {
                OpenItem(selectedItems[0]);
                return true;
            }
        }

        if (keyData == Keys.Back && GoUp())
        {
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void ShowContextMenuForKeyboard()
    {
        if (filesGrid.Focused && filesGrid.CurrentCell is not null)
        {
            var cellRectangle = filesGrid.GetCellDisplayRectangle(
                filesGrid.CurrentCell.ColumnIndex,
                filesGrid.CurrentCell.RowIndex,
                cutOverflow: true);
            browserContextMenu.Show(filesGrid, new Point(cellRectangle.Left, cellRectangle.Bottom));
            return;
        }

        browserContextMenu.Show(filesGrid, new Point(12, 12));
    }

    private bool GoBack()
    {
        if (navigationIndex <= 0)
        {
            return false;
        }

        navigationIndex--;
        NavigateTo(navigationHistory[navigationIndex], addToHistory: false);
        return true;
    }

    private bool GoForward()
    {
        if (navigationIndex < 0 || navigationIndex >= navigationHistory.Count - 1)
        {
            return false;
        }

        navigationIndex++;
        NavigateTo(navigationHistory[navigationIndex], addToHistory: false);
        return true;
    }

    private bool GoUp()
    {
        try
        {
            var currentPath = new DirectoryInfo(pathTextBox.Text);
            if (currentPath.Parent is null)
            {
                return false;
            }

            NavigateTo(currentPath.Parent.FullName, addToHistory: true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool IsTextInputFocused()
    {
        return ActiveControl is TextBoxBase or ComboBox;
    }

    private void FocusFileGrid()
    {
        if (filesGrid.Rows.Count > 0)
        {
            if (filesGrid.CurrentCell is null)
            {
                filesGrid.CurrentCell = filesGrid.Rows[0].Cells[0];
            }

            if (filesGrid.SelectedRows.Count == 0)
            {
                filesGrid.Rows[filesGrid.CurrentCell.RowIndex].Selected = true;
            }
        }

        filesGrid.Focus();
    }

    private void UpdateClipboardStatus()
    {
        if (clipboardIntent is null)
        {
            UpdateStatus("Clipboard cleared");
            return;
        }

        var verb = clipboardIntent.Action == ClipboardAction.Copy ? "Copied" : "Cut";
        UpdateStatus($"{verb} {clipboardIntent.Items.Count} item(s). Choose a destination and paste.");
    }

    private void filesGrid_SelectionChanged(object? sender, EventArgs e)
    {
        var selectedCount = filesGrid.SelectedRows.Count;
        if (selectedCount > 1)
        {
            UpdateStatus($"{selectedCount} item(s) selected");
        }
    }

    private void RemoveDeletedItemsFromClipboard(List<FileItemView> deletedItems)
    {
        if (clipboardIntent is null)
        {
            return;
        }

        var deletedPaths = deletedItems
            .Select(item => item.Path)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var remainingItems = clipboardIntent.Items
            .Where(item => !deletedPaths.Contains(item.SourcePath))
            .ToList();

        clipboardIntent = remainingItems.Count == 0
            ? null
            : clipboardIntent with { Items = remainingItems };
        UpdateClipboardStatus();
    }

    private void CompressSelectedItemsAsZip(List<FileItemView> selectedItems)
    {
        using var dialog = new SaveFileDialog
        {
            InitialDirectory = GetCurrentDirectoryPath() ?? string.Empty,
            Filter = "ZIP archive (*.zip)|*.zip",
            DefaultExt = "zip",
            FileName = "SelectedItems.zip"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var stagingRoot = Path.Combine(Path.GetTempPath(), "FileOptimizerBatchZip", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(stagingRoot);
            foreach (var item in selectedItems)
            {
                NativeMethods.CopyPath(item.Path, Path.Combine(stagingRoot, item.Name));
            }

            LogUi($"Compress batch as ZIP -> '{Path.GetFileName(dialog.FileName)}'.");
            NativeMethods.CompressPath(stagingRoot, dialog.FileName, CompressionFormat.Zip);
            UpdateStatus($"Compressed {selectedItems.Count} item(s) to {dialog.FileName}");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            try
            {
                if (Directory.Exists(stagingRoot))
                {
                    Directory.Delete(stagingRoot, recursive: true);
                }
            }
            catch
            {
                // Best-effort cleanup for temporary batch ZIP staging.
            }
        }
    }

    private sealed record ClipboardIntent(List<ClipboardItemIntent> Items, ClipboardAction Action);
    private sealed record ClipboardItemIntent(string SourcePath, string Name);

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
        LogUi($"Error: {message}");
        MessageBox.Show(this, message, "File Optimizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static void LogUi(string message)
    {
        AppConsole.Log("UI", message);
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }
}

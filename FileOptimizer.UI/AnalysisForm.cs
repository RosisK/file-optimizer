namespace FileOptimizer.UI;

public partial class AnalysisForm : Form
{
    public AnalysisForm(string initialDirectory, string? initialFile)
    {
        InitializeComponent();

        analysisPathTextBox.Text = Directory.Exists(initialDirectory)
            ? initialDirectory
            : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (!string.IsNullOrWhiteSpace(initialFile))
        {
            benchmarkFileTextBox.Text = initialFile;
        }

        iterationsNumericUpDown.Value = 5;
        sortKeyComboBox.DataSource = Enum.GetValues<SortOption>();
        sortKeyComboBox.SelectedItem = SortOption.Name;
    }

    private void browsePathButton_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            InitialDirectory = Directory.Exists(analysisPathTextBox.Text) ? analysisPathTextBox.Text : string.Empty
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            analysisPathTextBox.Text = dialog.SelectedPath;
        }
    }

    private void browseFileButton_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            InitialDirectory = Directory.Exists(analysisPathTextBox.Text) ? analysisPathTextBox.Text : string.Empty,
            Filter = "All files (*.*)|*.*"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            benchmarkFileTextBox.Text = dialog.FileName;
        }
    }

    private void storageButton_Click(object sender, EventArgs e)
    {
        RunAnalysis(() => AnalysisService.GetStorageSummary(GetAnalysisPath()));
    }

    private void searchBenchmarkButton_Click(object sender, EventArgs e)
    {
        RunAnalysis(() => AlgorithmLabService.RunSearchAlgorithmStudy(GetAnalysisPath(), searchQueryTextBox.Text.Trim(), GetIterations()));
    }

    private void sortBenchmarkButton_Click(object sender, EventArgs e)
    {
        RunAnalysis(() => AlgorithmLabService.RunSortAlgorithmStudy(GetAnalysisPath(), GetSortKey(), GetIterations()));
    }

    private void compressionBenchmarkButton_Click(object sender, EventArgs e)
    {
        RunAnalysis(() => AnalysisService.RunCompressionBenchmark(GetBenchmarkFile()));
    }

    private void duplicateNamesButton_Click(object sender, EventArgs e)
    {
        RunAnalysis(() => AnalysisService.RunDuplicateNameAnalysis(GetAnalysisPath()));
    }

    private void duplicateContentsButton_Click(object sender, EventArgs e)
    {
        RunAnalysis(() => AnalysisService.RunDuplicateContentAnalysis(GetAnalysisPath()));
    }

    private void clearResultsButton_Click(object sender, EventArgs e)
    {
        resultsTextBox.Clear();
        statusLabel.Text = "Results cleared";
    }

    private void RunAnalysis(Func<string> action)
    {
        try
        {
            statusLabel.Text = "Running analysis...";
            UseWaitCursor = true;
            Refresh();

            var result = action();
            AppendResult(result);
            statusLabel.Text = "Analysis complete";
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Analysis failed";
            MessageBox.Show(this, ex.Message, "Analysis", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    private void AppendResult(string result)
    {
        if (resultsTextBox.TextLength > 0)
        {
            resultsTextBox.AppendText(Environment.NewLine);
            resultsTextBox.AppendText(new string('-', 60));
            resultsTextBox.AppendText(Environment.NewLine);
        }

        resultsTextBox.AppendText(result);
        resultsTextBox.AppendText(Environment.NewLine);
    }

    private string GetAnalysisPath()
    {
        var path = analysisPathTextBox.Text.Trim();
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException("Choose a valid directory for analysis.");
        }

        return path;
    }

    private string GetBenchmarkFile()
    {
        var file = benchmarkFileTextBox.Text.Trim();
        if (!File.Exists(file))
        {
            throw new FileNotFoundException("Choose a valid file for the compression benchmark.", file);
        }

        return file;
    }

    private int GetIterations() => (int)iterationsNumericUpDown.Value;
    private SortOption GetSortKey() => sortKeyComboBox.SelectedItem is SortOption value ? value : SortOption.Name;
}

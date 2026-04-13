namespace FileOptimizer.UI;

partial class AnalysisForm
{
    private System.ComponentModel.IContainer components = null;
    private Label analysisPathLabel;
    private TextBox analysisPathTextBox;
    private Button browsePathButton;
    private Label searchQueryLabel;
    private TextBox searchQueryTextBox;
    private Label iterationsLabel;
    private NumericUpDown iterationsNumericUpDown;
    private Label benchmarkFileLabel;
    private TextBox benchmarkFileTextBox;
    private Button browseFileButton;
    private Button storageButton;
    private Button searchBenchmarkButton;
    private Button sortBenchmarkButton;
    private Button compressionBenchmarkButton;
    private Button clearResultsButton;
    private TextBox resultsTextBox;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusLabel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        analysisPathLabel = new Label();
        analysisPathTextBox = new TextBox();
        browsePathButton = new Button();
        searchQueryLabel = new Label();
        searchQueryTextBox = new TextBox();
        iterationsLabel = new Label();
        iterationsNumericUpDown = new NumericUpDown();
        benchmarkFileLabel = new Label();
        benchmarkFileTextBox = new TextBox();
        browseFileButton = new Button();
        storageButton = new Button();
        searchBenchmarkButton = new Button();
        sortBenchmarkButton = new Button();
        compressionBenchmarkButton = new Button();
        clearResultsButton = new Button();
        resultsTextBox = new TextBox();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        ((System.ComponentModel.ISupportInitialize)iterationsNumericUpDown).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // analysisPathLabel
        // 
        analysisPathLabel.AutoSize = true;
        analysisPathLabel.Location = new Point(12, 15);
        analysisPathLabel.Name = "analysisPathLabel";
        analysisPathLabel.Size = new Size(81, 15);
        analysisPathLabel.TabIndex = 0;
        analysisPathLabel.Text = "Analysis Path:";
        // 
        // analysisPathTextBox
        // 
        analysisPathTextBox.Location = new Point(99, 12);
        analysisPathTextBox.Name = "analysisPathTextBox";
        analysisPathTextBox.Size = new Size(500, 23);
        analysisPathTextBox.TabIndex = 1;
        // 
        // browsePathButton
        // 
        browsePathButton.Location = new Point(605, 11);
        browsePathButton.Name = "browsePathButton";
        browsePathButton.Size = new Size(82, 25);
        browsePathButton.TabIndex = 2;
        browsePathButton.Text = "Browse";
        browsePathButton.UseVisualStyleBackColor = true;
        browsePathButton.Click += browsePathButton_Click;
        // 
        // searchQueryLabel
        // 
        searchQueryLabel.AutoSize = true;
        searchQueryLabel.Location = new Point(12, 50);
        searchQueryLabel.Name = "searchQueryLabel";
        searchQueryLabel.Size = new Size(76, 15);
        searchQueryLabel.TabIndex = 3;
        searchQueryLabel.Text = "Search Query:";
        // 
        // searchQueryTextBox
        // 
        searchQueryTextBox.Location = new Point(99, 47);
        searchQueryTextBox.Name = "searchQueryTextBox";
        searchQueryTextBox.Size = new Size(220, 23);
        searchQueryTextBox.TabIndex = 4;
        // 
        // iterationsLabel
        // 
        iterationsLabel.AutoSize = true;
        iterationsLabel.Location = new Point(337, 50);
        iterationsLabel.Name = "iterationsLabel";
        iterationsLabel.Size = new Size(57, 15);
        iterationsLabel.TabIndex = 5;
        iterationsLabel.Text = "Iterations:";
        // 
        // iterationsNumericUpDown
        // 
        iterationsNumericUpDown.Location = new Point(400, 47);
        iterationsNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        iterationsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        iterationsNumericUpDown.Name = "iterationsNumericUpDown";
        iterationsNumericUpDown.Size = new Size(70, 23);
        iterationsNumericUpDown.TabIndex = 6;
        iterationsNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // benchmarkFileLabel
        // 
        benchmarkFileLabel.AutoSize = true;
        benchmarkFileLabel.Location = new Point(12, 85);
        benchmarkFileLabel.Name = "benchmarkFileLabel";
        benchmarkFileLabel.Size = new Size(84, 15);
        benchmarkFileLabel.TabIndex = 7;
        benchmarkFileLabel.Text = "Test File (Ratio):";
        // 
        // benchmarkFileTextBox
        // 
        benchmarkFileTextBox.Location = new Point(99, 82);
        benchmarkFileTextBox.Name = "benchmarkFileTextBox";
        benchmarkFileTextBox.Size = new Size(500, 23);
        benchmarkFileTextBox.TabIndex = 8;
        // 
        // browseFileButton
        // 
        browseFileButton.Location = new Point(605, 81);
        browseFileButton.Name = "browseFileButton";
        browseFileButton.Size = new Size(82, 25);
        browseFileButton.TabIndex = 9;
        browseFileButton.Text = "Choose File";
        browseFileButton.UseVisualStyleBackColor = true;
        browseFileButton.Click += browseFileButton_Click;
        // 
        // storageButton
        // 
        storageButton.Location = new Point(12, 123);
        storageButton.Name = "storageButton";
        storageButton.Size = new Size(115, 30);
        storageButton.TabIndex = 10;
        storageButton.Text = "Storage Usage";
        storageButton.UseVisualStyleBackColor = true;
        storageButton.Click += storageButton_Click;
        // 
        // searchBenchmarkButton
        // 
        searchBenchmarkButton.Location = new Point(133, 123);
        searchBenchmarkButton.Name = "searchBenchmarkButton";
        searchBenchmarkButton.Size = new Size(130, 30);
        searchBenchmarkButton.TabIndex = 11;
        searchBenchmarkButton.Text = "Search Benchmark";
        searchBenchmarkButton.UseVisualStyleBackColor = true;
        searchBenchmarkButton.Click += searchBenchmarkButton_Click;
        // 
        // sortBenchmarkButton
        // 
        sortBenchmarkButton.Location = new Point(269, 123);
        sortBenchmarkButton.Name = "sortBenchmarkButton";
        sortBenchmarkButton.Size = new Size(120, 30);
        sortBenchmarkButton.TabIndex = 12;
        sortBenchmarkButton.Text = "Sort Benchmark";
        sortBenchmarkButton.UseVisualStyleBackColor = true;
        sortBenchmarkButton.Click += sortBenchmarkButton_Click;
        // 
        // compressionBenchmarkButton
        // 
        compressionBenchmarkButton.Location = new Point(395, 123);
        compressionBenchmarkButton.Name = "compressionBenchmarkButton";
        compressionBenchmarkButton.Size = new Size(165, 30);
        compressionBenchmarkButton.TabIndex = 13;
        compressionBenchmarkButton.Text = "Compression Benchmark";
        compressionBenchmarkButton.UseVisualStyleBackColor = true;
        compressionBenchmarkButton.Click += compressionBenchmarkButton_Click;
        // 
        // clearResultsButton
        // 
        clearResultsButton.Location = new Point(566, 123);
        clearResultsButton.Name = "clearResultsButton";
        clearResultsButton.Size = new Size(121, 30);
        clearResultsButton.TabIndex = 14;
        clearResultsButton.Text = "Clear Results";
        clearResultsButton.UseVisualStyleBackColor = true;
        clearResultsButton.Click += clearResultsButton_Click;
        // 
        // resultsTextBox
        // 
        resultsTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        resultsTextBox.Location = new Point(12, 170);
        resultsTextBox.Multiline = true;
        resultsTextBox.Name = "resultsTextBox";
        resultsTextBox.ReadOnly = true;
        resultsTextBox.ScrollBars = ScrollBars.Vertical;
        resultsTextBox.Size = new Size(675, 322);
        resultsTextBox.TabIndex = 15;
        // 
        // statusStrip
        // 
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 501);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(699, 22);
        statusStrip.TabIndex = 16;
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(95, 17);
        statusLabel.Text = "Ready to analyze";
        // 
        // AnalysisForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(699, 523);
        Controls.Add(statusStrip);
        Controls.Add(resultsTextBox);
        Controls.Add(clearResultsButton);
        Controls.Add(compressionBenchmarkButton);
        Controls.Add(sortBenchmarkButton);
        Controls.Add(searchBenchmarkButton);
        Controls.Add(storageButton);
        Controls.Add(browseFileButton);
        Controls.Add(benchmarkFileTextBox);
        Controls.Add(benchmarkFileLabel);
        Controls.Add(iterationsNumericUpDown);
        Controls.Add(iterationsLabel);
        Controls.Add(searchQueryTextBox);
        Controls.Add(searchQueryLabel);
        Controls.Add(browsePathButton);
        Controls.Add(analysisPathTextBox);
        Controls.Add(analysisPathLabel);
        MinimumSize = new Size(715, 562);
        Name = "AnalysisForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Analysis Panel";
        ((System.ComponentModel.ISupportInitialize)iterationsNumericUpDown).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}

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
    private Label sortKeyLabel;
    private ComboBox sortKeyComboBox;
    private Label benchmarkFileLabel;
    private TextBox benchmarkFileTextBox;
    private Button browseFileButton;
    private Button storageButton;
    private Button searchBenchmarkButton;
    private Button sortBenchmarkButton;
    private Button duplicateNamesButton;
    private Button duplicateContentsButton;
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
        analysisPathLabel = new Label();
        analysisPathTextBox = new TextBox();
        browsePathButton = new Button();
        searchQueryLabel = new Label();
        searchQueryTextBox = new TextBox();
        iterationsLabel = new Label();
        iterationsNumericUpDown = new NumericUpDown();
        sortKeyLabel = new Label();
        sortKeyComboBox = new ComboBox();
        benchmarkFileLabel = new Label();
        benchmarkFileTextBox = new TextBox();
        browseFileButton = new Button();
        storageButton = new Button();
        searchBenchmarkButton = new Button();
        sortBenchmarkButton = new Button();
        duplicateNamesButton = new Button();
        duplicateContentsButton = new Button();
        clearResultsButton = new Button();
        resultsTextBox = new TextBox();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        compressionBenchmarkButton = new Button();
        ((System.ComponentModel.ISupportInitialize)iterationsNumericUpDown).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // analysisPathLabel
        // 
        analysisPathLabel.AutoSize = true;
        analysisPathLabel.Location = new Point(14, 20);
        analysisPathLabel.Name = "analysisPathLabel";
        analysisPathLabel.Size = new Size(97, 20);
        analysisPathLabel.TabIndex = 0;
        analysisPathLabel.Text = "Analysis Path:";
        // 
        // analysisPathTextBox
        // 
        analysisPathTextBox.Location = new Point(113, 16);
        analysisPathTextBox.Margin = new Padding(3, 4, 3, 4);
        analysisPathTextBox.Name = "analysisPathTextBox";
        analysisPathTextBox.Size = new Size(571, 27);
        analysisPathTextBox.TabIndex = 1;
        // 
        // browsePathButton
        // 
        browsePathButton.Location = new Point(691, 15);
        browsePathButton.Margin = new Padding(3, 4, 3, 4);
        browsePathButton.Name = "browsePathButton";
        browsePathButton.Size = new Size(94, 33);
        browsePathButton.TabIndex = 2;
        browsePathButton.Text = "Browse";
        browsePathButton.UseVisualStyleBackColor = true;
        browsePathButton.Click += browsePathButton_Click;
        // 
        // searchQueryLabel
        // 
        searchQueryLabel.AutoSize = true;
        searchQueryLabel.Location = new Point(14, 67);
        searchQueryLabel.Name = "searchQueryLabel";
        searchQueryLabel.Size = new Size(99, 20);
        searchQueryLabel.TabIndex = 3;
        searchQueryLabel.Text = "Search Query:";
        // 
        // searchQueryTextBox
        // 
        searchQueryTextBox.Location = new Point(113, 63);
        searchQueryTextBox.Margin = new Padding(3, 4, 3, 4);
        searchQueryTextBox.Name = "searchQueryTextBox";
        searchQueryTextBox.Size = new Size(251, 27);
        searchQueryTextBox.TabIndex = 4;
        // 
        // iterationsLabel
        // 
        iterationsLabel.AutoSize = true;
        iterationsLabel.Location = new Point(385, 67);
        iterationsLabel.Name = "iterationsLabel";
        iterationsLabel.Size = new Size(74, 20);
        iterationsLabel.TabIndex = 5;
        iterationsLabel.Text = "Iterations:";
        // 
        // iterationsNumericUpDown
        // 
        iterationsNumericUpDown.Location = new Point(457, 63);
        iterationsNumericUpDown.Margin = new Padding(3, 4, 3, 4);
        iterationsNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        iterationsNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        iterationsNumericUpDown.Name = "iterationsNumericUpDown";
        iterationsNumericUpDown.Size = new Size(80, 27);
        iterationsNumericUpDown.TabIndex = 6;
        iterationsNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // sortKeyLabel
        // 
        sortKeyLabel.AutoSize = true;
        sortKeyLabel.Location = new Point(561, 67);
        sortKeyLabel.Name = "sortKeyLabel";
        sortKeyLabel.Size = new Size(59, 20);
        sortKeyLabel.TabIndex = 7;
        sortKeyLabel.Text = "Sort By:";
        // 
        // sortKeyComboBox
        // 
        sortKeyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        sortKeyComboBox.FormattingEnabled = true;
        sortKeyComboBox.Location = new Point(622, 63);
        sortKeyComboBox.Margin = new Padding(3, 4, 3, 4);
        sortKeyComboBox.Name = "sortKeyComboBox";
        sortKeyComboBox.Size = new Size(163, 28);
        sortKeyComboBox.TabIndex = 8;
        // 
        // benchmarkFileLabel
        // 
        benchmarkFileLabel.AutoSize = true;
        benchmarkFileLabel.Location = new Point(14, 113);
        benchmarkFileLabel.Name = "benchmarkFileLabel";
        benchmarkFileLabel.Size = new Size(114, 20);
        benchmarkFileLabel.TabIndex = 9;
        benchmarkFileLabel.Text = "Test File (Ratio):";
        // 
        // benchmarkFileTextBox
        // 
        benchmarkFileTextBox.Location = new Point(113, 109);
        benchmarkFileTextBox.Margin = new Padding(3, 4, 3, 4);
        benchmarkFileTextBox.Name = "benchmarkFileTextBox";
        benchmarkFileTextBox.Size = new Size(571, 27);
        benchmarkFileTextBox.TabIndex = 10;
        // 
        // browseFileButton
        // 
        browseFileButton.Location = new Point(691, 108);
        browseFileButton.Margin = new Padding(3, 4, 3, 4);
        browseFileButton.Name = "browseFileButton";
        browseFileButton.Size = new Size(94, 33);
        browseFileButton.TabIndex = 11;
        browseFileButton.Text = "Choose File";
        browseFileButton.UseVisualStyleBackColor = true;
        browseFileButton.Click += browseFileButton_Click;
        // 
        // storageButton
        // 
        storageButton.Location = new Point(14, 164);
        storageButton.Margin = new Padding(3, 4, 3, 4);
        storageButton.Name = "storageButton";
        storageButton.Size = new Size(131, 40);
        storageButton.TabIndex = 12;
        storageButton.Text = "Storage Usage";
        storageButton.UseVisualStyleBackColor = true;
        storageButton.Click += storageButton_Click;
        // 
        // searchBenchmarkButton
        // 
        searchBenchmarkButton.Location = new Point(152, 164);
        searchBenchmarkButton.Margin = new Padding(3, 4, 3, 4);
        searchBenchmarkButton.Name = "searchBenchmarkButton";
        searchBenchmarkButton.Size = new Size(171, 40);
        searchBenchmarkButton.TabIndex = 13;
        searchBenchmarkButton.Text = "Search Algorithm Lab";
        searchBenchmarkButton.UseVisualStyleBackColor = true;
        searchBenchmarkButton.Click += searchBenchmarkButton_Click;
        // 
        // sortBenchmarkButton
        // 
        sortBenchmarkButton.Location = new Point(330, 164);
        sortBenchmarkButton.Margin = new Padding(3, 4, 3, 4);
        sortBenchmarkButton.Name = "sortBenchmarkButton";
        sortBenchmarkButton.Size = new Size(149, 40);
        sortBenchmarkButton.TabIndex = 14;
        sortBenchmarkButton.Text = "Sort Algorithm Lab";
        sortBenchmarkButton.UseVisualStyleBackColor = true;
        sortBenchmarkButton.Click += sortBenchmarkButton_Click;
        // 
        // duplicateNamesButton
        // 
        duplicateNamesButton.Location = new Point(14, 212);
        duplicateNamesButton.Margin = new Padding(3, 4, 3, 4);
        duplicateNamesButton.Name = "duplicateNamesButton";
        duplicateNamesButton.Size = new Size(194, 40);
        duplicateNamesButton.TabIndex = 16;
        duplicateNamesButton.Text = "Find Name Duplicates";
        duplicateNamesButton.UseVisualStyleBackColor = true;
        duplicateNamesButton.Click += duplicateNamesButton_Click;
        // 
        // duplicateContentsButton
        // 
        duplicateContentsButton.Location = new Point(215, 212);
        duplicateContentsButton.Margin = new Padding(3, 4, 3, 4);
        duplicateContentsButton.Name = "duplicateContentsButton";
        duplicateContentsButton.Size = new Size(211, 40);
        duplicateContentsButton.TabIndex = 17;
        duplicateContentsButton.Text = "Find Content Duplicates";
        duplicateContentsButton.UseVisualStyleBackColor = true;
        duplicateContentsButton.Click += duplicateContentsButton_Click;
        // 
        // clearResultsButton
        // 
        clearResultsButton.Location = new Point(647, 212);
        clearResultsButton.Margin = new Padding(3, 4, 3, 4);
        clearResultsButton.Name = "clearResultsButton";
        clearResultsButton.Size = new Size(138, 40);
        clearResultsButton.TabIndex = 18;
        clearResultsButton.Text = "Clear Results";
        clearResultsButton.UseVisualStyleBackColor = true;
        clearResultsButton.Click += clearResultsButton_Click;
        // 
        // resultsTextBox
        // 
        resultsTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        resultsTextBox.BackColor = Color.WhiteSmoke;
        resultsTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        resultsTextBox.Location = new Point(14, 273);
        resultsTextBox.Margin = new Padding(3, 4, 3, 4);
        resultsTextBox.Multiline = true;
        resultsTextBox.Name = "resultsTextBox";
        resultsTextBox.ReadOnly = true;
        resultsTextBox.ScrollBars = ScrollBars.Both;
        resultsTextBox.Size = new Size(771, 381);
        resultsTextBox.TabIndex = 17;
        resultsTextBox.WordWrap = false;
        // 
        // statusStrip
        // 
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 671);
        statusStrip.Name = "statusStrip";
        statusStrip.Padding = new Padding(1, 0, 16, 0);
        statusStrip.Size = new Size(799, 26);
        statusStrip.TabIndex = 16;
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(122, 20);
        statusLabel.Text = "Ready to analyze";
        // 
        // compressionBenchmarkButton
        // 
        compressionBenchmarkButton.Location = new Point(486, 164);
        compressionBenchmarkButton.Margin = new Padding(3, 4, 3, 4);
        compressionBenchmarkButton.Name = "compressionBenchmarkButton";
        compressionBenchmarkButton.Size = new Size(183, 40);
        compressionBenchmarkButton.TabIndex = 15;
        compressionBenchmarkButton.Text = "Compression Benchmark";
        compressionBenchmarkButton.UseVisualStyleBackColor = true;
        compressionBenchmarkButton.Click += compressionBenchmarkButton_Click;
        // 
        // AnalysisForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(799, 697);
        Controls.Add(statusStrip);
        Controls.Add(resultsTextBox);
        Controls.Add(clearResultsButton);
        Controls.Add(duplicateContentsButton);
        Controls.Add(duplicateNamesButton);
        Controls.Add(compressionBenchmarkButton);
        Controls.Add(sortBenchmarkButton);
        Controls.Add(searchBenchmarkButton);
        Controls.Add(storageButton);
        Controls.Add(browseFileButton);
        Controls.Add(benchmarkFileTextBox);
        Controls.Add(benchmarkFileLabel);
        Controls.Add(iterationsNumericUpDown);
        Controls.Add(iterationsLabel);
        Controls.Add(sortKeyComboBox);
        Controls.Add(sortKeyLabel);
        Controls.Add(searchQueryTextBox);
        Controls.Add(searchQueryLabel);
        Controls.Add(browsePathButton);
        Controls.Add(analysisPathTextBox);
        Controls.Add(analysisPathLabel);
        Margin = new Padding(3, 4, 3, 4);
        MinimumSize = new Size(815, 734);
        Name = "AnalysisForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Analysis Panel";
        ((System.ComponentModel.ISupportInitialize)iterationsNumericUpDown).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
    private Button compressionBenchmarkButton;
}

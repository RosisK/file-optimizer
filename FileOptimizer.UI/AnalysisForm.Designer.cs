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
    private Button compressionBenchmarkButton;
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
        components = new System.ComponentModel.Container();
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
        compressionBenchmarkButton = new Button();
        duplicateNamesButton = new Button();
        duplicateContentsButton = new Button();
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
        // sortKeyLabel
        // 
        sortKeyLabel.AutoSize = true;
        sortKeyLabel.Location = new Point(491, 50);
        sortKeyLabel.Name = "sortKeyLabel";
        sortKeyLabel.Size = new Size(47, 15);
        sortKeyLabel.TabIndex = 7;
        sortKeyLabel.Text = "Sort By:";
        // 
        // sortKeyComboBox
        // 
        sortKeyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        sortKeyComboBox.FormattingEnabled = true;
        sortKeyComboBox.Location = new Point(544, 47);
        sortKeyComboBox.Name = "sortKeyComboBox";
        sortKeyComboBox.Size = new Size(143, 23);
        sortKeyComboBox.TabIndex = 8;
        // 
        // benchmarkFileLabel
        // 
        benchmarkFileLabel.AutoSize = true;
        benchmarkFileLabel.Location = new Point(12, 85);
        benchmarkFileLabel.Name = "benchmarkFileLabel";
        benchmarkFileLabel.Size = new Size(84, 15);
        benchmarkFileLabel.TabIndex = 9;
        benchmarkFileLabel.Text = "Test File (Ratio):";
        // 
        // benchmarkFileTextBox
        // 
        benchmarkFileTextBox.Location = new Point(99, 82);
        benchmarkFileTextBox.Name = "benchmarkFileTextBox";
        benchmarkFileTextBox.Size = new Size(500, 23);
        benchmarkFileTextBox.TabIndex = 10;
        // 
        // browseFileButton
        // 
        browseFileButton.Location = new Point(605, 81);
        browseFileButton.Name = "browseFileButton";
        browseFileButton.Size = new Size(82, 25);
        browseFileButton.TabIndex = 11;
        browseFileButton.Text = "Choose File";
        browseFileButton.UseVisualStyleBackColor = true;
        browseFileButton.Click += browseFileButton_Click;
        // 
        // storageButton
        // 
        storageButton.Location = new Point(12, 123);
        storageButton.Name = "storageButton";
        storageButton.Size = new Size(115, 30);
        storageButton.TabIndex = 12;
        storageButton.Text = "Storage Usage";
        storageButton.UseVisualStyleBackColor = true;
        storageButton.Click += storageButton_Click;
        // 
        // searchBenchmarkButton
        // 
        searchBenchmarkButton.Location = new Point(133, 123);
        searchBenchmarkButton.Name = "searchBenchmarkButton";
        searchBenchmarkButton.Size = new Size(150, 30);
        searchBenchmarkButton.TabIndex = 13;
        searchBenchmarkButton.Text = "Search Algorithm Lab";
        searchBenchmarkButton.UseVisualStyleBackColor = true;
        searchBenchmarkButton.Click += searchBenchmarkButton_Click;
        // 
        // sortBenchmarkButton
        // 
        sortBenchmarkButton.Location = new Point(289, 123);
        sortBenchmarkButton.Name = "sortBenchmarkButton";
        sortBenchmarkButton.Size = new Size(130, 30);
        sortBenchmarkButton.TabIndex = 14;
        sortBenchmarkButton.Text = "Sort Algorithm Lab";
        sortBenchmarkButton.UseVisualStyleBackColor = true;
        sortBenchmarkButton.Click += sortBenchmarkButton_Click;
        // 
        // compressionBenchmarkButton
        // 
        compressionBenchmarkButton.Location = new Point(425, 123);
        compressionBenchmarkButton.Name = "compressionBenchmarkButton";
        compressionBenchmarkButton.Size = new Size(155, 30);
        compressionBenchmarkButton.TabIndex = 15;
        compressionBenchmarkButton.Text = "Compression Benchmark";
        compressionBenchmarkButton.UseVisualStyleBackColor = true;
        compressionBenchmarkButton.Click += compressionBenchmarkButton_Click;
        // 
        // duplicateNamesButton
        // 
        duplicateNamesButton.Location = new Point(12, 159);
        duplicateNamesButton.Name = "duplicateNamesButton";
        duplicateNamesButton.Size = new Size(170, 30);
        duplicateNamesButton.TabIndex = 16;
        duplicateNamesButton.Text = "Find Name Duplicates";
        duplicateNamesButton.UseVisualStyleBackColor = true;
        duplicateNamesButton.Click += duplicateNamesButton_Click;
        // 
        // duplicateContentsButton
        // 
        duplicateContentsButton.Location = new Point(188, 159);
        duplicateContentsButton.Name = "duplicateContentsButton";
        duplicateContentsButton.Size = new Size(185, 30);
        duplicateContentsButton.TabIndex = 17;
        duplicateContentsButton.Text = "Find Content Duplicates";
        duplicateContentsButton.UseVisualStyleBackColor = true;
        duplicateContentsButton.Click += duplicateContentsButton_Click;
        // 
        // clearResultsButton
        // 
        clearResultsButton.Location = new Point(566, 159);
        clearResultsButton.Name = "clearResultsButton";
        clearResultsButton.Size = new Size(121, 30);
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
        resultsTextBox.Location = new Point(12, 205);
        resultsTextBox.Multiline = true;
        resultsTextBox.Name = "resultsTextBox";
        resultsTextBox.ReadOnly = true;
        resultsTextBox.ScrollBars = ScrollBars.Both;
        resultsTextBox.Size = new Size(675, 287);
        resultsTextBox.TabIndex = 17;
        resultsTextBox.WordWrap = false;
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

namespace FileOptimizer.UI;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private System.Windows.Forms.Label pathLabel;
    private System.Windows.Forms.TextBox pathTextBox;
    private System.Windows.Forms.Button browseButton;
    private System.Windows.Forms.Button upButton;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Label searchLabel;
    private System.Windows.Forms.TextBox searchTextBox;
    private System.Windows.Forms.Button searchButton;
    private System.Windows.Forms.Button clearSearchButton;
    private System.Windows.Forms.ComboBox sortComboBox;
    private System.Windows.Forms.CheckBox ascendingCheckBox;
    private System.Windows.Forms.CheckBox directoriesFirstCheckBox;
    private System.Windows.Forms.DataGridView filesGrid;
    private System.Windows.Forms.Button copyButton;
    private System.Windows.Forms.Button cutButton;
    private System.Windows.Forms.Button pasteButton;
    private System.Windows.Forms.Button deleteButton;
    private System.Windows.Forms.Button compressButton;
    private System.Windows.Forms.Button decompressButton;
    private System.Windows.Forms.Button analysisButton;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel statusLabel;
    private System.Windows.Forms.DataGridViewTextBoxColumn typeColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn sizeColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn modifiedColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn pathColumn;

    private void InitializeComponent()
    {
        pathLabel = new Label();
        pathTextBox = new TextBox();
        browseButton = new Button();
        upButton = new Button();
        refreshButton = new Button();
        searchLabel = new Label();
        searchTextBox = new TextBox();
        searchButton = new Button();
        clearSearchButton = new Button();
        sortComboBox = new ComboBox();
        ascendingCheckBox = new CheckBox();
        directoriesFirstCheckBox = new CheckBox();
        filesGrid = new DataGridView();
        typeColumn = new DataGridViewTextBoxColumn();
        nameColumn = new DataGridViewTextBoxColumn();
        sizeColumn = new DataGridViewTextBoxColumn();
        modifiedColumn = new DataGridViewTextBoxColumn();
        pathColumn = new DataGridViewTextBoxColumn();
        copyButton = new Button();
        cutButton = new Button();
        pasteButton = new Button();
        deleteButton = new Button();
        compressButton = new Button();
        decompressButton = new Button();
        analysisButton = new Button();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        ((System.ComponentModel.ISupportInitialize)filesGrid).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // pathLabel
        // 
        pathLabel.AutoSize = true;
        pathLabel.Location = new Point(14, 20);
        pathLabel.Name = "pathLabel";
        pathLabel.Size = new Size(40, 20);
        pathLabel.TabIndex = 0;
        pathLabel.Text = "Path:";
        // 
        // pathTextBox
        // 
        pathTextBox.Location = new Point(59, 16);
        pathTextBox.Margin = new Padding(3, 4, 3, 4);
        pathTextBox.Name = "pathTextBox";
        pathTextBox.Size = new Size(722, 27);
        pathTextBox.TabIndex = 1;
        pathTextBox.KeyDown += pathTextBox_KeyDown;
        // 
        // browseButton
        // 
        browseButton.Location = new Point(789, 15);
        browseButton.Margin = new Padding(3, 4, 3, 4);
        browseButton.Name = "browseButton";
        browseButton.Size = new Size(97, 33);
        browseButton.TabIndex = 2;
        browseButton.Text = "Browse";
        browseButton.UseVisualStyleBackColor = true;
        browseButton.Click += browseButton_Click;
        // 
        // upButton
        // 
        upButton.Location = new Point(893, 15);
        upButton.Margin = new Padding(3, 4, 3, 4);
        upButton.Name = "upButton";
        upButton.Size = new Size(59, 33);
        upButton.TabIndex = 3;
        upButton.Text = "Up";
        upButton.UseVisualStyleBackColor = true;
        upButton.Click += upButton_Click;
        // 
        // refreshButton
        // 
        refreshButton.Location = new Point(959, 15);
        refreshButton.Margin = new Padding(3, 4, 3, 4);
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(86, 33);
        refreshButton.TabIndex = 4;
        refreshButton.Text = "Refresh";
        refreshButton.UseVisualStyleBackColor = true;
        refreshButton.Click += refreshButton_Click;
        // 
        // searchLabel
        // 
        searchLabel.AutoSize = true;
        searchLabel.Location = new Point(14, 68);
        searchLabel.Name = "searchLabel";
        searchLabel.Size = new Size(56, 20);
        searchLabel.TabIndex = 5;
        searchLabel.Text = "Search:";
        // 
        // searchTextBox
        // 
        searchTextBox.Location = new Point(72, 64);
        searchTextBox.Margin = new Padding(3, 4, 3, 4);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.Size = new Size(257, 27);
        searchTextBox.TabIndex = 6;
        searchTextBox.KeyDown += searchTextBox_KeyDown;
        // 
        // searchButton
        // 
        searchButton.Location = new Point(336, 63);
        searchButton.Margin = new Padding(3, 4, 3, 4);
        searchButton.Name = "searchButton";
        searchButton.Size = new Size(86, 33);
        searchButton.TabIndex = 7;
        searchButton.Text = "Search";
        searchButton.UseVisualStyleBackColor = true;
        searchButton.Click += searchButton_Click;
        // 
        // clearSearchButton
        // 
        clearSearchButton.Location = new Point(429, 63);
        clearSearchButton.Margin = new Padding(3, 4, 3, 4);
        clearSearchButton.Name = "clearSearchButton";
        clearSearchButton.Size = new Size(86, 33);
        clearSearchButton.TabIndex = 8;
        clearSearchButton.Text = "Clear";
        clearSearchButton.UseVisualStyleBackColor = true;
        clearSearchButton.Click += clearSearchButton_Click;
        // 
        // sortComboBox
        // 
        sortComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        sortComboBox.FormattingEnabled = true;
        sortComboBox.Location = new Point(608, 64);
        sortComboBox.Margin = new Padding(3, 4, 3, 4);
        sortComboBox.Name = "sortComboBox";
        sortComboBox.Size = new Size(138, 28);
        sortComboBox.TabIndex = 9;
        sortComboBox.SelectedIndexChanged += sortOptionsChanged;
        // 
        // ascendingCheckBox
        // 
        ascendingCheckBox.AutoSize = true;
        ascendingCheckBox.Checked = true;
        ascendingCheckBox.CheckState = CheckState.Checked;
        ascendingCheckBox.Location = new Point(765, 67);
        ascendingCheckBox.Margin = new Padding(3, 4, 3, 4);
        ascendingCheckBox.Name = "ascendingCheckBox";
        ascendingCheckBox.Size = new Size(100, 24);
        ascendingCheckBox.TabIndex = 10;
        ascendingCheckBox.Text = "Ascending";
        ascendingCheckBox.UseVisualStyleBackColor = true;
        ascendingCheckBox.CheckedChanged += sortOptionsChanged;
        // 
        // directoriesFirstCheckBox
        // 
        directoriesFirstCheckBox.AutoSize = true;
        directoriesFirstCheckBox.Checked = true;
        directoriesFirstCheckBox.CheckState = CheckState.Checked;
        directoriesFirstCheckBox.Location = new Point(873, 67);
        directoriesFirstCheckBox.Margin = new Padding(3, 4, 3, 4);
        directoriesFirstCheckBox.Name = "directoriesFirstCheckBox";
        directoriesFirstCheckBox.Size = new Size(132, 24);
        directoriesFirstCheckBox.TabIndex = 11;
        directoriesFirstCheckBox.Text = "Directories first";
        directoriesFirstCheckBox.UseVisualStyleBackColor = true;
        directoriesFirstCheckBox.CheckedChanged += sortOptionsChanged;
        // 
        // filesGrid
        // 
        filesGrid.AllowUserToAddRows = false;
        filesGrid.AllowUserToDeleteRows = false;
        filesGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        filesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        filesGrid.Columns.AddRange(new DataGridViewColumn[] { typeColumn, nameColumn, sizeColumn, modifiedColumn, pathColumn });
        filesGrid.Location = new Point(14, 116);
        filesGrid.Margin = new Padding(3, 4, 3, 4);
        filesGrid.MultiSelect = true;
        filesGrid.Name = "filesGrid";
        filesGrid.ReadOnly = true;
        filesGrid.RowHeadersVisible = false;
        filesGrid.RowHeadersWidth = 51;
        filesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        filesGrid.Size = new Size(1031, 525);
        filesGrid.TabIndex = 12;
        filesGrid.CellDoubleClick += filesGrid_CellDoubleClick;
        // 
        // typeColumn
        // 
        typeColumn.DataPropertyName = "Type";
        typeColumn.HeaderText = "Type";
        typeColumn.MinimumWidth = 6;
        typeColumn.Name = "typeColumn";
        typeColumn.ReadOnly = true;
        typeColumn.Width = 75;
        // 
        // nameColumn
        // 
        nameColumn.DataPropertyName = "Name";
        nameColumn.HeaderText = "Name";
        nameColumn.MinimumWidth = 6;
        nameColumn.Name = "nameColumn";
        nameColumn.ReadOnly = true;
        nameColumn.Width = 180;
        // 
        // sizeColumn
        // 
        sizeColumn.DataPropertyName = "DisplaySize";
        sizeColumn.HeaderText = "Size";
        sizeColumn.MinimumWidth = 6;
        sizeColumn.Name = "sizeColumn";
        sizeColumn.ReadOnly = true;
        sizeColumn.Width = 120;
        // 
        // modifiedColumn
        // 
        modifiedColumn.DataPropertyName = "Modified";
        modifiedColumn.HeaderText = "Modified";
        modifiedColumn.MinimumWidth = 6;
        modifiedColumn.Name = "modifiedColumn";
        modifiedColumn.ReadOnly = true;
        modifiedColumn.Width = 150;
        // 
        // pathColumn
        // 
        pathColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        pathColumn.DataPropertyName = "Path";
        pathColumn.HeaderText = "Path";
        pathColumn.MinimumWidth = 6;
        pathColumn.Name = "pathColumn";
        pathColumn.ReadOnly = true;
        // 
        // copyButton
        // 
        copyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        copyButton.Location = new Point(14, 656);
        copyButton.Margin = new Padding(3, 4, 3, 4);
        copyButton.Name = "copyButton";
        copyButton.Size = new Size(103, 40);
        copyButton.TabIndex = 13;
        copyButton.Text = "Copy";
        copyButton.UseVisualStyleBackColor = true;
        copyButton.Click += copyButton_Click;
        // 
        // cutButton
        // 
        cutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        cutButton.Location = new Point(123, 656);
        cutButton.Margin = new Padding(3, 4, 3, 4);
        cutButton.Name = "cutButton";
        cutButton.Size = new Size(103, 40);
        cutButton.TabIndex = 14;
        cutButton.Text = "Cut";
        cutButton.UseVisualStyleBackColor = true;
        cutButton.Click += cutButton_Click;
        // 
        // pasteButton
        // 
        pasteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        pasteButton.Location = new Point(233, 656);
        pasteButton.Margin = new Padding(3, 4, 3, 4);
        pasteButton.Name = "pasteButton";
        pasteButton.Size = new Size(103, 40);
        pasteButton.TabIndex = 15;
        pasteButton.Text = "Paste";
        pasteButton.UseVisualStyleBackColor = true;
        pasteButton.Click += pasteButton_Click;
        // 
        // deleteButton
        // 
        deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        deleteButton.Location = new Point(343, 656);
        deleteButton.Margin = new Padding(3, 4, 3, 4);
        deleteButton.Name = "deleteButton";
        deleteButton.Size = new Size(103, 40);
        deleteButton.TabIndex = 16;
        deleteButton.Text = "Delete";
        deleteButton.UseVisualStyleBackColor = true;
        deleteButton.Click += deleteButton_Click;
        // 
        // compressButton
        // 
        compressButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        compressButton.Location = new Point(453, 656);
        compressButton.Margin = new Padding(3, 4, 3, 4);
        compressButton.Name = "compressButton";
        compressButton.Size = new Size(114, 40);
        compressButton.TabIndex = 17;
        compressButton.Text = "Compress";
        compressButton.UseVisualStyleBackColor = true;
        compressButton.Click += compressButton_Click;
        // 
        // decompressButton
        // 
        decompressButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        decompressButton.Location = new Point(574, 656);
        decompressButton.Margin = new Padding(3, 4, 3, 4);
        decompressButton.Name = "decompressButton";
        decompressButton.Size = new Size(126, 40);
        decompressButton.TabIndex = 18;
        decompressButton.Text = "Decompress";
        decompressButton.UseVisualStyleBackColor = true;
        decompressButton.Click += decompressButton_Click;
        // 
        // analysisButton
        // 
        analysisButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        analysisButton.Location = new Point(706, 656);
        analysisButton.Margin = new Padding(3, 4, 3, 4);
        analysisButton.Name = "analysisButton";
        analysisButton.Size = new Size(126, 40);
        analysisButton.TabIndex = 19;
        analysisButton.Text = "Analysis";
        analysisButton.UseVisualStyleBackColor = true;
        analysisButton.Click += analysisButton_Click;
        // 
        // statusStrip
        // 
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 714);
        statusStrip.Name = "statusStrip";
        statusStrip.Padding = new Padding(1, 0, 16, 0);
        statusStrip.Size = new Size(1058, 26);
        statusStrip.TabIndex = 18;
        statusStrip.Text = "statusStrip1";
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(90, 20);
        statusLabel.Text = "Ready to go";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1058, 740);
        Controls.Add(statusStrip);
        Controls.Add(analysisButton);
        Controls.Add(decompressButton);
        Controls.Add(compressButton);
        Controls.Add(deleteButton);
        Controls.Add(pasteButton);
        Controls.Add(cutButton);
        Controls.Add(copyButton);
        Controls.Add(filesGrid);
        Controls.Add(directoriesFirstCheckBox);
        Controls.Add(ascendingCheckBox);
        Controls.Add(sortComboBox);
        Controls.Add(clearSearchButton);
        Controls.Add(searchButton);
        Controls.Add(searchTextBox);
        Controls.Add(searchLabel);
        Controls.Add(refreshButton);
        Controls.Add(upButton);
        Controls.Add(browseButton);
        Controls.Add(pathTextBox);
        Controls.Add(pathLabel);
        Margin = new Padding(3, 4, 3, 4);
        MinimumSize = new Size(1074, 776);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "File Pilot Desktop";
        Load += Form1_Load;
        ((System.ComponentModel.ISupportInitialize)filesGrid).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}

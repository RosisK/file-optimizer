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
        this.components = new System.ComponentModel.Container();
        this.pathLabel = new System.Windows.Forms.Label();
        this.pathTextBox = new System.Windows.Forms.TextBox();
        this.browseButton = new System.Windows.Forms.Button();
        this.upButton = new System.Windows.Forms.Button();
        this.refreshButton = new System.Windows.Forms.Button();
        this.searchLabel = new System.Windows.Forms.Label();
        this.searchTextBox = new System.Windows.Forms.TextBox();
        this.searchButton = new System.Windows.Forms.Button();
        this.clearSearchButton = new System.Windows.Forms.Button();
        this.sortComboBox = new System.Windows.Forms.ComboBox();
        this.ascendingCheckBox = new System.Windows.Forms.CheckBox();
        this.directoriesFirstCheckBox = new System.Windows.Forms.CheckBox();
        this.filesGrid = new System.Windows.Forms.DataGridView();
        this.typeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.sizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.modifiedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.pathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.copyButton = new System.Windows.Forms.Button();
        this.cutButton = new System.Windows.Forms.Button();
        this.pasteButton = new System.Windows.Forms.Button();
        this.deleteButton = new System.Windows.Forms.Button();
        this.compressButton = new System.Windows.Forms.Button();
        this.decompressButton = new System.Windows.Forms.Button();
        this.analysisButton = new System.Windows.Forms.Button();
        this.statusStrip = new System.Windows.Forms.StatusStrip();
        this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
        ((System.ComponentModel.ISupportInitialize)(this.filesGrid)).BeginInit();
        this.statusStrip.SuspendLayout();
        this.SuspendLayout();
        // 
        // pathLabel
        // 
        this.pathLabel.AutoSize = true;
        this.pathLabel.Location = new System.Drawing.Point(12, 15);
        this.pathLabel.Name = "pathLabel";
        this.pathLabel.Size = new System.Drawing.Size(34, 15);
        this.pathLabel.TabIndex = 0;
        this.pathLabel.Text = "Path:";
        // 
        // pathTextBox
        // 
        this.pathTextBox.Location = new System.Drawing.Point(52, 12);
        this.pathTextBox.Name = "pathTextBox";
        this.pathTextBox.Size = new System.Drawing.Size(632, 23);
        this.pathTextBox.TabIndex = 1;
        this.pathTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathTextBox_KeyDown);
        // 
        // browseButton
        // 
        this.browseButton.Location = new System.Drawing.Point(690, 11);
        this.browseButton.Name = "browseButton";
        this.browseButton.Size = new System.Drawing.Size(85, 25);
        this.browseButton.TabIndex = 2;
        this.browseButton.Text = "Browse";
        this.browseButton.UseVisualStyleBackColor = true;
        this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
        // 
        // upButton
        // 
        this.upButton.Location = new System.Drawing.Point(781, 11);
        this.upButton.Name = "upButton";
        this.upButton.Size = new System.Drawing.Size(52, 25);
        this.upButton.TabIndex = 3;
        this.upButton.Text = "Up";
        this.upButton.UseVisualStyleBackColor = true;
        this.upButton.Click += new System.EventHandler(this.upButton_Click);
        // 
        // refreshButton
        // 
        this.refreshButton.Location = new System.Drawing.Point(839, 11);
        this.refreshButton.Name = "refreshButton";
        this.refreshButton.Size = new System.Drawing.Size(75, 25);
        this.refreshButton.TabIndex = 4;
        this.refreshButton.Text = "Refresh";
        this.refreshButton.UseVisualStyleBackColor = true;
        this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
        // 
        // searchLabel
        // 
        this.searchLabel.AutoSize = true;
        this.searchLabel.Location = new System.Drawing.Point(12, 51);
        this.searchLabel.Name = "searchLabel";
        this.searchLabel.Size = new System.Drawing.Size(45, 15);
        this.searchLabel.TabIndex = 5;
        this.searchLabel.Text = "Search:";
        // 
        // searchTextBox
        // 
        this.searchTextBox.Location = new System.Drawing.Point(63, 48);
        this.searchTextBox.Name = "searchTextBox";
        this.searchTextBox.Size = new System.Drawing.Size(225, 23);
        this.searchTextBox.TabIndex = 6;
        this.searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchTextBox_KeyDown);
        // 
        // searchButton
        // 
        this.searchButton.Location = new System.Drawing.Point(294, 47);
        this.searchButton.Name = "searchButton";
        this.searchButton.Size = new System.Drawing.Size(75, 25);
        this.searchButton.TabIndex = 7;
        this.searchButton.Text = "Search";
        this.searchButton.UseVisualStyleBackColor = true;
        this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
        // 
        // clearSearchButton
        // 
        this.clearSearchButton.Location = new System.Drawing.Point(375, 47);
        this.clearSearchButton.Name = "clearSearchButton";
        this.clearSearchButton.Size = new System.Drawing.Size(75, 25);
        this.clearSearchButton.TabIndex = 8;
        this.clearSearchButton.Text = "Clear";
        this.clearSearchButton.UseVisualStyleBackColor = true;
        this.clearSearchButton.Click += new System.EventHandler(this.clearSearchButton_Click);
        // 
        // sortComboBox
        // 
        this.sortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.sortComboBox.FormattingEnabled = true;
        this.sortComboBox.Location = new System.Drawing.Point(532, 48);
        this.sortComboBox.Name = "sortComboBox";
        this.sortComboBox.Size = new System.Drawing.Size(121, 23);
        this.sortComboBox.TabIndex = 9;
        this.sortComboBox.SelectedIndexChanged += new System.EventHandler(this.sortOptionsChanged);
        // 
        // ascendingCheckBox
        // 
        this.ascendingCheckBox.AutoSize = true;
        this.ascendingCheckBox.Checked = true;
        this.ascendingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        this.ascendingCheckBox.Location = new System.Drawing.Point(669, 50);
        this.ascendingCheckBox.Name = "ascendingCheckBox";
        this.ascendingCheckBox.Size = new System.Drawing.Size(79, 19);
        this.ascendingCheckBox.TabIndex = 10;
        this.ascendingCheckBox.Text = "Ascending";
        this.ascendingCheckBox.UseVisualStyleBackColor = true;
        this.ascendingCheckBox.CheckedChanged += new System.EventHandler(this.sortOptionsChanged);
        // 
        // directoriesFirstCheckBox
        // 
        this.directoriesFirstCheckBox.AutoSize = true;
        this.directoriesFirstCheckBox.Checked = true;
        this.directoriesFirstCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        this.directoriesFirstCheckBox.Location = new System.Drawing.Point(764, 50);
        this.directoriesFirstCheckBox.Name = "directoriesFirstCheckBox";
        this.directoriesFirstCheckBox.Size = new System.Drawing.Size(110, 19);
        this.directoriesFirstCheckBox.TabIndex = 11;
        this.directoriesFirstCheckBox.Text = "Directories first";
        this.directoriesFirstCheckBox.UseVisualStyleBackColor = true;
        this.directoriesFirstCheckBox.CheckedChanged += new System.EventHandler(this.sortOptionsChanged);
        // 
        // filesGrid
        // 
        this.filesGrid.AllowUserToAddRows = false;
        this.filesGrid.AllowUserToDeleteRows = false;
        this.filesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
        this.filesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.filesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.typeColumn,
            this.nameColumn,
            this.sizeColumn,
            this.modifiedColumn,
            this.pathColumn});
        this.filesGrid.Location = new System.Drawing.Point(12, 87);
        this.filesGrid.MultiSelect = false;
        this.filesGrid.Name = "filesGrid";
        this.filesGrid.ReadOnly = true;
        this.filesGrid.RowHeadersVisible = false;
        this.filesGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.filesGrid.Size = new System.Drawing.Size(902, 394);
        this.filesGrid.TabIndex = 12;
        this.filesGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.filesGrid_CellDoubleClick);
        // 
        // typeColumn
        // 
        this.typeColumn.DataPropertyName = "Type";
        this.typeColumn.HeaderText = "Type";
        this.typeColumn.Name = "typeColumn";
        this.typeColumn.ReadOnly = true;
        this.typeColumn.Width = 75;
        // 
        // nameColumn
        // 
        this.nameColumn.DataPropertyName = "Name";
        this.nameColumn.HeaderText = "Name";
        this.nameColumn.Name = "nameColumn";
        this.nameColumn.ReadOnly = true;
        this.nameColumn.Width = 180;
        // 
        // sizeColumn
        // 
        this.sizeColumn.DataPropertyName = "DisplaySize";
        this.sizeColumn.HeaderText = "Size";
        this.sizeColumn.Name = "sizeColumn";
        this.sizeColumn.ReadOnly = true;
        this.sizeColumn.Width = 120;
        // 
        // modifiedColumn
        // 
        this.modifiedColumn.DataPropertyName = "Modified";
        this.modifiedColumn.HeaderText = "Modified";
        this.modifiedColumn.Name = "modifiedColumn";
        this.modifiedColumn.ReadOnly = true;
        this.modifiedColumn.Width = 150;
        // 
        // pathColumn
        // 
        this.pathColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
        this.pathColumn.DataPropertyName = "Path";
        this.pathColumn.HeaderText = "Path";
        this.pathColumn.Name = "pathColumn";
        this.pathColumn.ReadOnly = true;
        // 
        // copyButton
        // 
        this.copyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.copyButton.Location = new System.Drawing.Point(12, 492);
        this.copyButton.Name = "copyButton";
        this.copyButton.Size = new System.Drawing.Size(90, 30);
        this.copyButton.TabIndex = 13;
        this.copyButton.Text = "Copy";
        this.copyButton.UseVisualStyleBackColor = true;
        this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
        // 
        // cutButton
        // 
        this.cutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.cutButton.Location = new System.Drawing.Point(108, 492);
        this.cutButton.Name = "cutButton";
        this.cutButton.Size = new System.Drawing.Size(90, 30);
        this.cutButton.TabIndex = 14;
        this.cutButton.Text = "Cut";
        this.cutButton.UseVisualStyleBackColor = true;
        this.cutButton.Click += new System.EventHandler(this.cutButton_Click);
        // 
        // pasteButton
        // 
        this.pasteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.pasteButton.Location = new System.Drawing.Point(204, 492);
        this.pasteButton.Name = "pasteButton";
        this.pasteButton.Size = new System.Drawing.Size(90, 30);
        this.pasteButton.TabIndex = 15;
        this.pasteButton.Text = "Paste";
        this.pasteButton.UseVisualStyleBackColor = true;
        this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
        // 
        // deleteButton
        // 
        this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.deleteButton.Location = new System.Drawing.Point(300, 492);
        this.deleteButton.Name = "deleteButton";
        this.deleteButton.Size = new System.Drawing.Size(90, 30);
        this.deleteButton.TabIndex = 16;
        this.deleteButton.Text = "Delete";
        this.deleteButton.UseVisualStyleBackColor = true;
        this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
        // 
        // compressButton
        // 
        this.compressButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.compressButton.Location = new System.Drawing.Point(396, 492);
        this.compressButton.Name = "compressButton";
        this.compressButton.Size = new System.Drawing.Size(100, 30);
        this.compressButton.TabIndex = 17;
        this.compressButton.Text = "Compress";
        this.compressButton.UseVisualStyleBackColor = true;
        this.compressButton.Click += new System.EventHandler(this.compressButton_Click);
        // 
        // decompressButton
        // 
        this.decompressButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.decompressButton.Location = new System.Drawing.Point(502, 492);
        this.decompressButton.Name = "decompressButton";
        this.decompressButton.Size = new System.Drawing.Size(110, 30);
        this.decompressButton.TabIndex = 18;
        this.decompressButton.Text = "Decompress";
        this.decompressButton.UseVisualStyleBackColor = true;
        this.decompressButton.Click += new System.EventHandler(this.decompressButton_Click);
        // 
        // analysisButton
        // 
        this.analysisButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.analysisButton.Location = new System.Drawing.Point(618, 492);
        this.analysisButton.Name = "analysisButton";
        this.analysisButton.Size = new System.Drawing.Size(110, 30);
        this.analysisButton.TabIndex = 19;
        this.analysisButton.Text = "Analysis";
        this.analysisButton.UseVisualStyleBackColor = true;
        this.analysisButton.Click += new System.EventHandler(this.analysisButton_Click);
        // 
        // statusStrip
        // 
        this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
        this.statusStrip.Location = new System.Drawing.Point(0, 533);
        this.statusStrip.Name = "statusStrip";
        this.statusStrip.Size = new System.Drawing.Size(926, 22);
        this.statusStrip.TabIndex = 18;
        this.statusStrip.Text = "statusStrip1";
        // 
        // statusLabel
        // 
        this.statusLabel.Name = "statusLabel";
        this.statusLabel.Size = new System.Drawing.Size(71, 17);
        this.statusLabel.Text = "Ready to go";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(926, 555);
        this.Controls.Add(this.statusStrip);
        this.Controls.Add(this.analysisButton);
        this.Controls.Add(this.decompressButton);
        this.Controls.Add(this.compressButton);
        this.Controls.Add(this.deleteButton);
        this.Controls.Add(this.pasteButton);
        this.Controls.Add(this.cutButton);
        this.Controls.Add(this.copyButton);
        this.Controls.Add(this.filesGrid);
        this.Controls.Add(this.directoriesFirstCheckBox);
        this.Controls.Add(this.ascendingCheckBox);
        this.Controls.Add(this.sortComboBox);
        this.Controls.Add(this.clearSearchButton);
        this.Controls.Add(this.searchButton);
        this.Controls.Add(this.searchTextBox);
        this.Controls.Add(this.searchLabel);
        this.Controls.Add(this.refreshButton);
        this.Controls.Add(this.upButton);
        this.Controls.Add(this.browseButton);
        this.Controls.Add(this.pathTextBox);
        this.Controls.Add(this.pathLabel);
        this.MinimumSize = new System.Drawing.Size(942, 594);
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "File Optimizer Desktop";
        ((System.ComponentModel.ISupportInitialize)(this.filesGrid)).EndInit();
        this.statusStrip.ResumeLayout(false);
        this.statusStrip.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
}

namespace DocSharp {
    partial class DocSharp {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.menu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.sourceInfo = new System.Windows.Forms.TreeView();
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.leftSplit = new System.Windows.Forms.SplitContainer();
            this.phpFillers = new System.Windows.Forms.CheckBox();
            this.extension = new System.Windows.Forms.ComboBox();
            this.extensionLabel = new System.Windows.Forms.Label();
            this.exportAttributes = new System.Windows.Forms.CheckBox();
            this.expandStructs = new System.Windows.Forms.CheckBox();
            this.structureLabel = new System.Windows.Forms.Label();
            this.expandEnums = new System.Windows.Forms.CheckBox();
            this.generateButton = new System.Windows.Forms.Button();
            this.settingsLabel = new System.Windows.Forms.Label();
            this.visibilityLabel = new System.Windows.Forms.Label();
            this.exportPrivate = new System.Windows.Forms.CheckBox();
            this.exportProtected = new System.Windows.Forms.CheckBox();
            this.exportInternal = new System.Windows.Forms.CheckBox();
            this.exportPublic = new System.Windows.Forms.CheckBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.definesLabel = new System.Windows.Forms.Label();
            this.defines = new System.Windows.Forms.TextBox();
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftSplit)).BeginInit();
            this.leftSplit.Panel1.SuspendLayout();
            this.leftSplit.Panel2.SuspendLayout();
            this.leftSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(838, 24);
            this.menu.TabIndex = 0;
            this.menu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSourceToolStripMenuItem,
            this.loadRecentToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadSourceToolStripMenuItem
            // 
            this.loadSourceToolStripMenuItem.Name = "loadSourceToolStripMenuItem";
            this.loadSourceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadSourceToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.loadSourceToolStripMenuItem.Text = "Load source";
            this.loadSourceToolStripMenuItem.Click += new System.EventHandler(this.loadSourceToolStripMenuItem_Click);
            // 
            // loadRecentToolStripMenuItem
            // 
            this.loadRecentToolStripMenuItem.Name = "loadRecentToolStripMenuItem";
            this.loadRecentToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.loadRecentToolStripMenuItem.Text = "Load recent";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // sourceInfo
            // 
            this.sourceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceInfo.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.sourceInfo.Location = new System.Drawing.Point(0, 0);
            this.sourceInfo.Name = "sourceInfo";
            this.sourceInfo.Size = new System.Drawing.Size(534, 451);
            this.sourceInfo.TabIndex = 1;
            this.sourceInfo.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sourceInfo_AfterSelect);
            // 
            // mainSplit
            // 
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainSplit.IsSplitterFixed = true;
            this.mainSplit.Location = new System.Drawing.Point(0, 24);
            this.mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            this.mainSplit.Panel1.Controls.Add(this.leftSplit);
            // 
            // mainSplit.Panel2
            // 
            this.mainSplit.Panel2.Controls.Add(this.sourceInfo);
            this.mainSplit.Size = new System.Drawing.Size(838, 451);
            this.mainSplit.SplitterDistance = 300;
            this.mainSplit.TabIndex = 2;
            // 
            // leftSplit
            // 
            this.leftSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.leftSplit.IsSplitterFixed = true;
            this.leftSplit.Location = new System.Drawing.Point(0, 0);
            this.leftSplit.Name = "leftSplit";
            this.leftSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // leftSplit.Panel1
            // 
            this.leftSplit.Panel1.Controls.Add(this.defines);
            this.leftSplit.Panel1.Controls.Add(this.definesLabel);
            this.leftSplit.Panel1.Controls.Add(this.phpFillers);
            this.leftSplit.Panel1.Controls.Add(this.extension);
            this.leftSplit.Panel1.Controls.Add(this.extensionLabel);
            this.leftSplit.Panel1.Controls.Add(this.exportAttributes);
            this.leftSplit.Panel1.Controls.Add(this.expandStructs);
            this.leftSplit.Panel1.Controls.Add(this.structureLabel);
            this.leftSplit.Panel1.Controls.Add(this.expandEnums);
            this.leftSplit.Panel1.Controls.Add(this.generateButton);
            this.leftSplit.Panel1.Controls.Add(this.settingsLabel);
            this.leftSplit.Panel1.Controls.Add(this.visibilityLabel);
            this.leftSplit.Panel1.Controls.Add(this.exportPrivate);
            this.leftSplit.Panel1.Controls.Add(this.exportProtected);
            this.leftSplit.Panel1.Controls.Add(this.exportInternal);
            this.leftSplit.Panel1.Controls.Add(this.exportPublic);
            // 
            // leftSplit.Panel2
            // 
            this.leftSplit.Panel2.Controls.Add(this.infoLabel);
            this.leftSplit.Panel2.Padding = new System.Windows.Forms.Padding(12);
            this.leftSplit.Size = new System.Drawing.Size(300, 451);
            this.leftSplit.SplitterDistance = 227;
            this.leftSplit.TabIndex = 0;
            // 
            // phpFillers
            // 
            this.phpFillers.AutoSize = true;
            this.phpFillers.Location = new System.Drawing.Point(15, 201);
            this.phpFillers.Name = "phpFillers";
            this.phpFillers.Size = new System.Drawing.Size(98, 17);
            this.phpFillers.TabIndex = 1136;
            this.phpFillers.Text = "index.php fillers";
            this.phpFillers.UseVisualStyleBackColor = true;
            // 
            // extension
            // 
            this.extension.FormattingEnabled = true;
            this.extension.Items.AddRange(new object[] {
            "html",
            "htm",
            "php"});
            this.extension.Location = new System.Drawing.Point(108, 174);
            this.extension.Name = "extension";
            this.extension.Size = new System.Drawing.Size(121, 21);
            this.extension.TabIndex = 1135;
            this.extension.Text = "html";
            // 
            // extensionLabel
            // 
            this.extensionLabel.AutoSize = true;
            this.extensionLabel.Location = new System.Drawing.Point(12, 177);
            this.extensionLabel.Name = "extensionLabel";
            this.extensionLabel.Size = new System.Drawing.Size(74, 13);
            this.extensionLabel.TabIndex = 11;
            this.extensionLabel.Text = "File extension:";
            // 
            // exportAttributes
            // 
            this.exportAttributes.AutoSize = true;
            this.exportAttributes.Location = new System.Drawing.Point(148, 102);
            this.exportAttributes.Name = "exportAttributes";
            this.exportAttributes.Size = new System.Drawing.Size(102, 17);
            this.exportAttributes.TabIndex = 6;
            this.exportAttributes.Text = "Export attributes";
            this.exportAttributes.UseVisualStyleBackColor = true;
            // 
            // expandStructs
            // 
            this.expandStructs.AutoSize = true;
            this.expandStructs.Checked = true;
            this.expandStructs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.expandStructs.Location = new System.Drawing.Point(148, 79);
            this.expandStructs.Name = "expandStructs";
            this.expandStructs.Size = new System.Drawing.Size(96, 17);
            this.expandStructs.TabIndex = 5;
            this.expandStructs.Text = "Expand structs";
            this.expandStructs.UseVisualStyleBackColor = true;
            // 
            // structureLabel
            // 
            this.structureLabel.AutoSize = true;
            this.structureLabel.Location = new System.Drawing.Point(145, 34);
            this.structureLabel.Name = "structureLabel";
            this.structureLabel.Size = new System.Drawing.Size(50, 13);
            this.structureLabel.TabIndex = 8;
            this.structureLabel.Text = "Structure";
            // 
            // expandEnums
            // 
            this.expandEnums.AutoSize = true;
            this.expandEnums.Checked = true;
            this.expandEnums.CheckState = System.Windows.Forms.CheckState.Checked;
            this.expandEnums.Location = new System.Drawing.Point(148, 56);
            this.expandEnums.Name = "expandEnums";
            this.expandEnums.Size = new System.Drawing.Size(96, 17);
            this.expandEnums.TabIndex = 4;
            this.expandEnums.Text = "Expand enums";
            this.expandEnums.UseVisualStyleBackColor = true;
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(197, 201);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(100, 23);
            this.generateButton.TabIndex = 1137;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // settingsLabel
            // 
            this.settingsLabel.AutoSize = true;
            this.settingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsLabel.Location = new System.Drawing.Point(12, 12);
            this.settingsLabel.Name = "settingsLabel";
            this.settingsLabel.Size = new System.Drawing.Size(117, 13);
            this.settingsLabel.TabIndex = 5;
            this.settingsLabel.Text = "Generation settings";
            // 
            // visibilityLabel
            // 
            this.visibilityLabel.AutoSize = true;
            this.visibilityLabel.Location = new System.Drawing.Point(12, 34);
            this.visibilityLabel.Name = "visibilityLabel";
            this.visibilityLabel.Size = new System.Drawing.Size(43, 13);
            this.visibilityLabel.TabIndex = 4;
            this.visibilityLabel.Text = "Visibility";
            // 
            // exportPrivate
            // 
            this.exportPrivate.AutoSize = true;
            this.exportPrivate.Location = new System.Drawing.Point(15, 125);
            this.exportPrivate.Name = "exportPrivate";
            this.exportPrivate.Size = new System.Drawing.Size(59, 17);
            this.exportPrivate.TabIndex = 3;
            this.exportPrivate.Text = "Private";
            this.exportPrivate.UseVisualStyleBackColor = true;
            // 
            // exportProtected
            // 
            this.exportProtected.AutoSize = true;
            this.exportProtected.Checked = true;
            this.exportProtected.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportProtected.Location = new System.Drawing.Point(15, 102);
            this.exportProtected.Name = "exportProtected";
            this.exportProtected.Size = new System.Drawing.Size(72, 17);
            this.exportProtected.TabIndex = 2;
            this.exportProtected.Text = "Protected";
            this.exportProtected.UseVisualStyleBackColor = true;
            // 
            // exportInternal
            // 
            this.exportInternal.AutoSize = true;
            this.exportInternal.Location = new System.Drawing.Point(15, 79);
            this.exportInternal.Name = "exportInternal";
            this.exportInternal.Size = new System.Drawing.Size(61, 17);
            this.exportInternal.TabIndex = 1;
            this.exportInternal.Text = "Internal";
            this.exportInternal.UseVisualStyleBackColor = true;
            // 
            // exportPublic
            // 
            this.exportPublic.AutoSize = true;
            this.exportPublic.Checked = true;
            this.exportPublic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportPublic.Location = new System.Drawing.Point(15, 56);
            this.exportPublic.Name = "exportPublic";
            this.exportPublic.Size = new System.Drawing.Size(55, 17);
            this.exportPublic.TabIndex = 0;
            this.exportPublic.Text = "Public";
            this.exportPublic.UseVisualStyleBackColor = true;
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoLabel.Location = new System.Drawing.Point(12, 12);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(0, 13);
            this.infoLabel.TabIndex = 0;
            // 
            // definesLabel
            // 
            this.definesLabel.AutoSize = true;
            this.definesLabel.Location = new System.Drawing.Point(12, 151);
            this.definesLabel.Name = "definesLabel";
            this.definesLabel.Size = new System.Drawing.Size(90, 13);
            this.definesLabel.TabIndex = 1139;
            this.definesLabel.Text = "Define constants:";
            // 
            // defines
            // 
            this.defines.Location = new System.Drawing.Point(108, 148);
            this.defines.Name = "defines";
            this.defines.Size = new System.Drawing.Size(188, 20);
            this.defines.TabIndex = 1134;
            this.defines.Text = "RELEASE;TRACE";
            // 
            // DocSharp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 475);
            this.Controls.Add(this.mainSplit);
            this.Controls.Add(this.menu);
            this.MainMenuStrip = this.menu;
            this.Name = "DocSharp";
            this.Text = "Doc#";
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.leftSplit.Panel1.ResumeLayout(false);
            this.leftSplit.Panel1.PerformLayout();
            this.leftSplit.Panel2.ResumeLayout(false);
            this.leftSplit.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftSplit)).EndInit();
            this.leftSplit.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSourceToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.TreeView sourceInfo;
        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.SplitContainer leftSplit;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.CheckBox exportInternal;
        private System.Windows.Forms.CheckBox exportPublic;
        private System.Windows.Forms.CheckBox exportProtected;
        private System.Windows.Forms.CheckBox exportPrivate;
        private System.Windows.Forms.Label visibilityLabel;
        private System.Windows.Forms.Label settingsLabel;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.Label structureLabel;
        private System.Windows.Forms.CheckBox expandEnums;
        private System.Windows.Forms.CheckBox expandStructs;
        private System.Windows.Forms.CheckBox exportAttributes;
        private System.Windows.Forms.ToolStripMenuItem loadRecentToolStripMenuItem;
        private System.Windows.Forms.Label extensionLabel;
        private System.Windows.Forms.ComboBox extension;
        private System.Windows.Forms.CheckBox phpFillers;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label definesLabel;
        private System.Windows.Forms.TextBox defines;
    }
}


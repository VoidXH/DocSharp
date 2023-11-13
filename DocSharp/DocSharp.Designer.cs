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
            menu = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            sourceInfo = new System.Windows.Forms.TreeView();
            mainSplit = new System.Windows.Forms.SplitContainer();
            leftSplit = new System.Windows.Forms.SplitContainer();
            ignore = new System.Windows.Forms.TextBox();
            ignoreLabel = new System.Windows.Forms.Label();
            gitignore = new System.Windows.Forms.CheckBox();
            definesBox = new System.Windows.Forms.GroupBox();
            defines = new System.Windows.Forms.TextBox();
            currentDefines = new System.Windows.Forms.Label();
            reloadConstants = new System.Windows.Forms.Button();
            extensionLabel = new System.Windows.Forms.Label();
            phpFillers = new System.Windows.Forms.CheckBox();
            exportAttributes = new System.Windows.Forms.CheckBox();
            generateButton = new System.Windows.Forms.Button();
            expandStructs = new System.Windows.Forms.CheckBox();
            extension = new System.Windows.Forms.ComboBox();
            structureLabel = new System.Windows.Forms.Label();
            expandEnums = new System.Windows.Forms.CheckBox();
            settingsLabel = new System.Windows.Forms.Label();
            visibilityLabel = new System.Windows.Forms.Label();
            exportPrivate = new System.Windows.Forms.CheckBox();
            exportProtected = new System.Windows.Forms.CheckBox();
            exportInternal = new System.Windows.Forms.CheckBox();
            exportPublic = new System.Windows.Forms.CheckBox();
            infoLabel = new System.Windows.Forms.Label();
            statusbar = new System.Windows.Forms.StatusStrip();
            progressBar = new System.Windows.Forms.ToolStripProgressBar();
            progressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplit).BeginInit();
            mainSplit.Panel1.SuspendLayout();
            mainSplit.Panel2.SuspendLayout();
            mainSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)leftSplit).BeginInit();
            leftSplit.Panel1.SuspendLayout();
            leftSplit.Panel2.SuspendLayout();
            leftSplit.SuspendLayout();
            definesBox.SuspendLayout();
            statusbar.SuspendLayout();
            SuspendLayout();
            // 
            // menu
            // 
            menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menu.Location = new System.Drawing.Point(0, 0);
            menu.Name = "menu";
            menu.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menu.Size = new System.Drawing.Size(961, 24);
            menu.TabIndex = 0;
            menu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { loadSourceToolStripMenuItem, loadRecentToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadSourceToolStripMenuItem
            // 
            loadSourceToolStripMenuItem.Name = "loadSourceToolStripMenuItem";
            loadSourceToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            loadSourceToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            loadSourceToolStripMenuItem.Text = "Load source";
            loadSourceToolStripMenuItem.Click += LoadSourceToolStripMenuItem_Click;
            // 
            // loadRecentToolStripMenuItem
            // 
            loadRecentToolStripMenuItem.Name = "loadRecentToolStripMenuItem";
            loadRecentToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            loadRecentToolStripMenuItem.Text = "Load recent";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // sourceInfo
            // 
            sourceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            sourceInfo.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            sourceInfo.Location = new System.Drawing.Point(0, 0);
            sourceInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            sourceInfo.Name = "sourceInfo";
            sourceInfo.Size = new System.Drawing.Size(636, 527);
            sourceInfo.TabIndex = 1;
            sourceInfo.AfterSelect += SourceInfo_AfterSelect;
            // 
            // mainSplit
            // 
            mainSplit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            mainSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            mainSplit.IsSplitterFixed = true;
            mainSplit.Location = new System.Drawing.Point(0, 24);
            mainSplit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            mainSplit.Panel1.Controls.Add(leftSplit);
            // 
            // mainSplit.Panel2
            // 
            mainSplit.Panel2.Controls.Add(sourceInfo);
            mainSplit.Size = new System.Drawing.Size(961, 527);
            mainSplit.SplitterDistance = 320;
            mainSplit.SplitterWidth = 5;
            mainSplit.TabIndex = 2;
            // 
            // leftSplit
            // 
            leftSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            leftSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            leftSplit.IsSplitterFixed = true;
            leftSplit.Location = new System.Drawing.Point(0, 0);
            leftSplit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            leftSplit.Name = "leftSplit";
            leftSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // leftSplit.Panel1
            // 
            leftSplit.Panel1.Controls.Add(ignore);
            leftSplit.Panel1.Controls.Add(ignoreLabel);
            leftSplit.Panel1.Controls.Add(gitignore);
            leftSplit.Panel1.Controls.Add(definesBox);
            leftSplit.Panel1.Controls.Add(extensionLabel);
            leftSplit.Panel1.Controls.Add(phpFillers);
            leftSplit.Panel1.Controls.Add(exportAttributes);
            leftSplit.Panel1.Controls.Add(generateButton);
            leftSplit.Panel1.Controls.Add(expandStructs);
            leftSplit.Panel1.Controls.Add(extension);
            leftSplit.Panel1.Controls.Add(structureLabel);
            leftSplit.Panel1.Controls.Add(expandEnums);
            leftSplit.Panel1.Controls.Add(settingsLabel);
            leftSplit.Panel1.Controls.Add(visibilityLabel);
            leftSplit.Panel1.Controls.Add(exportPrivate);
            leftSplit.Panel1.Controls.Add(exportProtected);
            leftSplit.Panel1.Controls.Add(exportInternal);
            leftSplit.Panel1.Controls.Add(exportPublic);
            // 
            // leftSplit.Panel2
            // 
            leftSplit.Panel2.Controls.Add(infoLabel);
            leftSplit.Panel2.Padding = new System.Windows.Forms.Padding(14);
            leftSplit.Size = new System.Drawing.Size(320, 527);
            leftSplit.SplitterDistance = 360;
            leftSplit.SplitterWidth = 5;
            leftSplit.TabIndex = 0;
            // 
            // ignore
            // 
            ignore.Location = new System.Drawing.Point(137, 297);
            ignore.Name = "ignore";
            ignore.Size = new System.Drawing.Size(173, 23);
            ignore.TabIndex = 10;
            // 
            // ignoreLabel
            // 
            ignoreLabel.AutoSize = true;
            ignoreLabel.Location = new System.Drawing.Point(10, 300);
            ignoreLabel.Name = "ignoreLabel";
            ignoreLabel.Size = new System.Drawing.Size(121, 15);
            ignoreLabel.TabIndex = 1140;
            ignoreLabel.Text = "Ignore (separator is ;):";
            // 
            // gitignore
            // 
            gitignore.AutoSize = true;
            gitignore.Location = new System.Drawing.Point(18, 326);
            gitignore.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gitignore.Name = "gitignore";
            gitignore.Size = new System.Drawing.Size(121, 19);
            gitignore.TabIndex = 11;
            gitignore.Text = "Respect .gitignore";
            gitignore.UseVisualStyleBackColor = true;
            // 
            // definesBox
            // 
            definesBox.Controls.Add(defines);
            definesBox.Controls.Add(currentDefines);
            definesBox.Controls.Add(reloadConstants);
            definesBox.Location = new System.Drawing.Point(10, 151);
            definesBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            definesBox.Name = "definesBox";
            definesBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            definesBox.Size = new System.Drawing.Size(300, 80);
            definesBox.TabIndex = 7;
            definesBox.TabStop = false;
            definesBox.Text = "Define constants";
            // 
            // defines
            // 
            defines.Location = new System.Drawing.Point(8, 22);
            defines.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            defines.Name = "defines";
            defines.Size = new System.Drawing.Size(284, 23);
            defines.TabIndex = 1;
            defines.Text = "RELEASE; MASTER";
            // 
            // currentDefines
            // 
            currentDefines.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            currentDefines.Location = new System.Drawing.Point(9, 53);
            currentDefines.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            currentDefines.Name = "currentDefines";
            currentDefines.Size = new System.Drawing.Size(203, 21);
            currentDefines.TabIndex = 1140;
            currentDefines.Text = "Code loaded with: -";
            // 
            // reloadConstants
            // 
            reloadConstants.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            reloadConstants.Location = new System.Drawing.Point(220, 47);
            reloadConstants.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            reloadConstants.Name = "reloadConstants";
            reloadConstants.Size = new System.Drawing.Size(72, 27);
            reloadConstants.TabIndex = 2;
            reloadConstants.Text = "Reload";
            reloadConstants.UseVisualStyleBackColor = true;
            reloadConstants.Click += ReloadConstants_Click;
            // 
            // extensionLabel
            // 
            extensionLabel.AutoSize = true;
            extensionLabel.Location = new System.Drawing.Point(10, 243);
            extensionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            extensionLabel.Name = "extensionLabel";
            extensionLabel.Size = new System.Drawing.Size(82, 15);
            extensionLabel.TabIndex = 11;
            extensionLabel.Text = "File extension:";
            // 
            // phpFillers
            // 
            phpFillers.AutoSize = true;
            phpFillers.Location = new System.Drawing.Point(18, 269);
            phpFillers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            phpFillers.Name = "phpFillers";
            phpFillers.Size = new System.Drawing.Size(110, 19);
            phpFillers.TabIndex = 9;
            phpFillers.Text = "index.php fillers";
            phpFillers.UseVisualStyleBackColor = true;
            // 
            // exportAttributes
            // 
            exportAttributes.AutoSize = true;
            exportAttributes.Location = new System.Drawing.Point(173, 101);
            exportAttributes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            exportAttributes.Name = "exportAttributes";
            exportAttributes.Size = new System.Drawing.Size(113, 19);
            exportAttributes.TabIndex = 6;
            exportAttributes.Text = "Export attributes";
            exportAttributes.UseVisualStyleBackColor = true;
            // 
            // generateButton
            // 
            generateButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            generateButton.Location = new System.Drawing.Point(193, 326);
            generateButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            generateButton.Name = "generateButton";
            generateButton.Size = new System.Drawing.Size(117, 27);
            generateButton.TabIndex = 1138;
            generateButton.Text = "Generate";
            generateButton.UseVisualStyleBackColor = true;
            generateButton.Click += GenerateButton_Click;
            // 
            // expandStructs
            // 
            expandStructs.AutoSize = true;
            expandStructs.Checked = true;
            expandStructs.CheckState = System.Windows.Forms.CheckState.Checked;
            expandStructs.Location = new System.Drawing.Point(173, 76);
            expandStructs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            expandStructs.Name = "expandStructs";
            expandStructs.Size = new System.Drawing.Size(103, 19);
            expandStructs.TabIndex = 5;
            expandStructs.Text = "Expand structs";
            expandStructs.UseVisualStyleBackColor = true;
            // 
            // extension
            // 
            extension.FormattingEnabled = true;
            extension.Items.AddRange(new object[] { "html", "htm", "php" });
            extension.Location = new System.Drawing.Point(100, 240);
            extension.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            extension.Name = "extension";
            extension.Size = new System.Drawing.Size(140, 23);
            extension.TabIndex = 8;
            extension.Text = "html";
            // 
            // structureLabel
            // 
            structureLabel.AutoSize = true;
            structureLabel.Location = new System.Drawing.Point(160, 33);
            structureLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            structureLabel.Name = "structureLabel";
            structureLabel.Size = new System.Drawing.Size(55, 15);
            structureLabel.TabIndex = 8;
            structureLabel.Text = "Structure";
            // 
            // expandEnums
            // 
            expandEnums.AutoSize = true;
            expandEnums.Checked = true;
            expandEnums.CheckState = System.Windows.Forms.CheckState.Checked;
            expandEnums.Location = new System.Drawing.Point(173, 51);
            expandEnums.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            expandEnums.Name = "expandEnums";
            expandEnums.Size = new System.Drawing.Size(104, 19);
            expandEnums.TabIndex = 4;
            expandEnums.Text = "Expand enums";
            expandEnums.UseVisualStyleBackColor = true;
            // 
            // settingsLabel
            // 
            settingsLabel.AutoSize = true;
            settingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            settingsLabel.Location = new System.Drawing.Point(10, 10);
            settingsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            settingsLabel.Name = "settingsLabel";
            settingsLabel.Size = new System.Drawing.Size(117, 13);
            settingsLabel.TabIndex = 5;
            settingsLabel.Text = "Generation settings";
            // 
            // visibilityLabel
            // 
            visibilityLabel.AutoSize = true;
            visibilityLabel.Location = new System.Drawing.Point(10, 33);
            visibilityLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            visibilityLabel.Name = "visibilityLabel";
            visibilityLabel.Size = new System.Drawing.Size(51, 15);
            visibilityLabel.TabIndex = 4;
            visibilityLabel.Text = "Visibility";
            // 
            // exportPrivate
            // 
            exportPrivate.AutoSize = true;
            exportPrivate.Location = new System.Drawing.Point(18, 126);
            exportPrivate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            exportPrivate.Name = "exportPrivate";
            exportPrivate.Size = new System.Drawing.Size(62, 19);
            exportPrivate.TabIndex = 3;
            exportPrivate.Text = "Private";
            exportPrivate.UseVisualStyleBackColor = true;
            // 
            // exportProtected
            // 
            exportProtected.AutoSize = true;
            exportProtected.Checked = true;
            exportProtected.CheckState = System.Windows.Forms.CheckState.Checked;
            exportProtected.Location = new System.Drawing.Point(18, 101);
            exportProtected.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            exportProtected.Name = "exportProtected";
            exportProtected.Size = new System.Drawing.Size(77, 19);
            exportProtected.TabIndex = 2;
            exportProtected.Text = "Protected";
            exportProtected.UseVisualStyleBackColor = true;
            // 
            // exportInternal
            // 
            exportInternal.AutoSize = true;
            exportInternal.Location = new System.Drawing.Point(18, 76);
            exportInternal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            exportInternal.Name = "exportInternal";
            exportInternal.Size = new System.Drawing.Size(66, 19);
            exportInternal.TabIndex = 1;
            exportInternal.Text = "Internal";
            exportInternal.UseVisualStyleBackColor = true;
            // 
            // exportPublic
            // 
            exportPublic.AutoSize = true;
            exportPublic.Checked = true;
            exportPublic.CheckState = System.Windows.Forms.CheckState.Checked;
            exportPublic.Location = new System.Drawing.Point(18, 51);
            exportPublic.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            exportPublic.Name = "exportPublic";
            exportPublic.Size = new System.Drawing.Size(59, 19);
            exportPublic.TabIndex = 0;
            exportPublic.Text = "Public";
            exportPublic.UseVisualStyleBackColor = true;
            // 
            // infoLabel
            // 
            infoLabel.AutoSize = true;
            infoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            infoLabel.Location = new System.Drawing.Point(14, 14);
            infoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            infoLabel.MaximumSize = new System.Drawing.Size(322, 493);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new System.Drawing.Size(0, 15);
            infoLabel.TabIndex = 0;
            // 
            // statusbar
            // 
            statusbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { progressBar, progressLabel });
            statusbar.Location = new System.Drawing.Point(0, 554);
            statusbar.Name = "statusbar";
            statusbar.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusbar.Size = new System.Drawing.Size(961, 24);
            statusbar.TabIndex = 3;
            statusbar.Text = "statusStrip1";
            // 
            // progressBar
            // 
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(117, 18);
            progressBar.Step = 1;
            // 
            // progressLabel
            // 
            progressLabel.Name = "progressLabel";
            progressLabel.Size = new System.Drawing.Size(0, 19);
            // 
            // DocSharp
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(961, 578);
            Controls.Add(statusbar);
            Controls.Add(mainSplit);
            Controls.Add(menu);
            MainMenuStrip = menu;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "DocSharp";
            Text = "Doc#";
            FormClosed += DocSharp_FormClosed;
            menu.ResumeLayout(false);
            menu.PerformLayout();
            mainSplit.Panel1.ResumeLayout(false);
            mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplit).EndInit();
            mainSplit.ResumeLayout(false);
            leftSplit.Panel1.ResumeLayout(false);
            leftSplit.Panel1.PerformLayout();
            leftSplit.Panel2.ResumeLayout(false);
            leftSplit.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)leftSplit).EndInit();
            leftSplit.ResumeLayout(false);
            definesBox.ResumeLayout(false);
            definesBox.PerformLayout();
            statusbar.ResumeLayout(false);
            statusbar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.TextBox defines;
        private System.Windows.Forms.Button reloadConstants;
        private System.Windows.Forms.Label currentDefines;
        private System.Windows.Forms.GroupBox definesBox;
        private System.Windows.Forms.StatusStrip statusbar;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel progressLabel;
        private System.Windows.Forms.CheckBox gitignore;
        private System.Windows.Forms.Label ignoreLabel;
        private System.Windows.Forms.TextBox ignore;
    }
}


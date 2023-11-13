using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// The main window of Doc#.
    /// </summary>
    public partial class DocSharp : Form {
        readonly TaskEngine task;

        string lastLoaded = string.Empty;

        MemberNode import;

        /// <summary>
        /// Load code from a folder.
        /// </summary>
        void LoadRecent(string recent) {
            string newRecents = Properties.Settings.Default.Recents;
            if (newRecents.Length == 0) {
                Properties.Settings.Default.Recents = recent;
            } else {
                int recentPos = newRecents.IndexOf(recent);
                Properties.Settings.Default.Recents = recentPos == -1 ? $"{recent}\n{newRecents}" : string.Format("{0}\n{1}{2}", recent,
                    newRecents[..recentPos], newRecents[(recentPos + recent.Length + 1)..]);
            }
            Properties.Settings.Default.Save();
            LoadRecents();
            LoadFrom(recent, defines.Text);
        }

        /// <summary>
        /// Load code from a folder which is on the recents list.
        /// </summary>
        void LoadRecent(object sender, EventArgs e) => LoadRecent(((ToolStripMenuItem)sender).Text);

        /// <summary>
        /// Fill the recents list with the saved entries.
        /// </summary>
        void LoadRecents() {
            loadRecentToolStripMenuItem.DropDownItems.Clear();
            string[] recents = Properties.Settings.Default.Recents.Split('\n');
            for (int i = 0; i < recents.Length - 1; ++i) {
                ToolStripItem recent = new ToolStripMenuItem(recents[i]);
                if (Directory.Exists(recents[i])) {
                    recent.Click += LoadRecent;
                } else {
                    recent.Enabled = false;
                }
                loadRecentToolStripMenuItem.DropDownItems.Add(recent);
            }
        }

        /// <summary>
        /// Load settings and set up the window accordingly.
        /// </summary>
        public DocSharp() {
            InitializeComponent();
            task = new TaskEngine();
            task.SetProgressReporting(progressBar, progressLabel);
            LoadRecents();
            expandEnums.Checked = Properties.Settings.Default.ExpandEnums;
            expandStructs.Checked = Properties.Settings.Default.ExpandStructs;
            exportAttributes.Checked = Properties.Settings.Default.ExportAttributes;
            extension.Text = Properties.Settings.Default.FileExtension;
            defines.Text = Properties.Settings.Default.DefineConstants;
            phpFillers.Checked = Properties.Settings.Default.PhpFillers;
            gitignore.Checked = Properties.Settings.Default.gitignore;
            ignore.Text = Properties.Settings.Default.ignore;
            exportInternal.Checked = Properties.Settings.Default.VisibilityInternal;
            exportPrivate.Checked = Properties.Settings.Default.VisibilityPrivate;
            exportProtected.Checked = Properties.Settings.Default.VisibilityProtected;
            exportPublic.Checked = Properties.Settings.Default.VisibilityPublic;
        }

        /// <summary>
        /// Save settings when Doc# is closed.
        /// </summary>
        void DocSharp_FormClosed(object _, FormClosedEventArgs e) {
            Properties.Settings.Default.ExpandEnums = expandEnums.Checked;
            Properties.Settings.Default.ExpandStructs = expandStructs.Checked;
            Properties.Settings.Default.ExportAttributes = exportAttributes.Checked;
            Properties.Settings.Default.FileExtension = extension.Text;
            Properties.Settings.Default.DefineConstants = defines.Text;
            Properties.Settings.Default.PhpFillers = phpFillers.Checked;
            Properties.Settings.Default.gitignore = gitignore.Checked;
            Properties.Settings.Default.ignore = ignore.Text;
            Properties.Settings.Default.VisibilityInternal = exportInternal.Checked;
            Properties.Settings.Default.VisibilityPrivate = exportPrivate.Checked;
            Properties.Settings.Default.VisibilityProtected = exportProtected.Checked;
            Properties.Settings.Default.VisibilityPublic = exportPublic.Checked;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Load code from a directory.
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <param name="defines">Defined constants</param>
        void LoadFrom(string path, string defines) {
            if (task.IsOperationRunning) {
                MessageBox.Show("Another operation is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(path)) {
                MessageBox.Show("The selected directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            lastLoaded = path;
            currentDefines.Text = "Code loaded with: " + (defines.Equals(string.Empty) ? "-" : defines);
            sourceInfo.BeginUpdate();
            sourceInfo.Nodes.Clear();
            import = MemberNode.MakeNamespace(path[(path.LastIndexOf('\\') + 1)..]);
            sourceInfo.Nodes.Add(import);

            List<string> excluded = new();
            if (!string.IsNullOrEmpty(ignore.Text)) {
                excluded.AddRange(ignore.Text.Split(';'));
            }
            string gitignorePath = Path.Combine(path, ".gitignore");
            if (gitignore.Checked && File.Exists(gitignorePath)) {
                excluded.AddRange(File.ReadAllLines(gitignorePath));
            }
            Parser parser = new(path, import, sourceInfo, defines, excluded.ToArray(), task);
            task.Run(parser.Process);
        }

        /// <summary>
        /// Reload the code with new constants
        /// </summary>
        void ReloadConstants_Click(object _, EventArgs e) => LoadFrom(lastLoaded, defines.Text);

        /// <summary>
        /// Open the directory picker for loading code.
        /// </summary>
        void LoadSourceToolStripMenuItem_Click(object _, EventArgs e) {
            if (folderDialog.ShowDialog() == DialogResult.OK) {
                LoadFrom(folderDialog.SelectedPath, defines.Text);
                if (Properties.Settings.Default.Recents.Contains(folderDialog.SelectedPath + '\n')) {
                    LoadRecent(folderDialog.SelectedPath);
                } else {
                    string newRecents = folderDialog.SelectedPath + '\n' + Properties.Settings.Default.Recents;
                    while (newRecents.Count(c => c == '\n') > 10) {
                        newRecents = newRecents[..newRecents.LastIndexOf('\n')];
                    }
                    Properties.Settings.Default.Recents = newRecents;
                    Properties.Settings.Default.Save();
                    LoadRecents();
                }
            }
        }

        /// <summary>
        /// Display the selected code element's properties.
        /// </summary>
        private void SourceInfo_AfterSelect(object _, TreeViewEventArgs e) {
            StringBuilder info = new();
            MemberNode node = (MemberNode)sourceInfo.SelectedNode;
            if (node != null && node.name != null) {
                Utils.AppendIfExists(info, "Attributes", node.attributes);
                Utils.AppendIfExists(info, "Visibility", node.Vis.ToString().ToLower());
                if (node.Kind == Element.Properties) {
                    Utils.AppendIfExists(info, "Getter", node.Getter.ToString().ToLower());
                    Utils.AppendIfExists(info, "Setter", node.Setter.ToString().ToLower());
                }
                Utils.AppendIfExists(info, "Modifiers", node.modifiers);
                Utils.AppendIfExists(info, "Type", HttpUtility.HtmlEncode(node.type));
                Utils.AppendIfExists(info, "Default value", node.defaultValue);
                Utils.AppendIfExists(info, "Extends", node.extends);
                if (node.summary.Length != 0) {
                    info.AppendLine().AppendLine().Append(Utils.QuickSummary(node.summary));
                }
            }
            infoLabel.Text = info.ToString();
        }

        /// <summary>
        /// Start documentation generation process.
        /// </summary>
        void GenerateButton_Click(object _, EventArgs e) {
            if (import == null) {
                MessageBox.Show("Please load a source first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (task.IsOperationRunning) {
                MessageBox.Show("Another operation is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (folderDialog.ShowDialog() == DialogResult.OK) {
                DirectoryInfo dir = new(folderDialog.SelectedPath);
                if ((dir.GetDirectories().Length == 0 && dir.GetDirectories().Length == 0) ||
                    MessageBox.Show(string.Format("The folder ({0}) is not empty. Continue generation anyway?",
                    folderDialog.SelectedPath), "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    Design.extension = extension.Text.Equals(string.Empty) ? "html" : extension.Text;
                    Design.exportAttributes = exportAttributes.Checked;
                    Exporter exporter = new(task) {
                        GenerateFillers = phpFillers.Checked && !extension.Text.Equals("php"),
                        ExportPublic = exportPublic.Checked,
                        ExportInternal = exportInternal.Checked,
                        ExportProtected = exportProtected.Checked,
                        ExportPrivate = exportPrivate.Checked,
                        ExpandEnums = expandEnums.Checked,
                        ExpandStructs = expandStructs.Checked
                    };
                    exporter.GenerateDocumentation(import, folderDialog.SelectedPath);
                }
            }
        }

        /// <summary>
        /// Show "About".
        /// </summary>
        void AboutToolStripMenuItem_Click(object _, EventArgs e) =>
            MessageBox.Show(string.Format("Doc# v1.1 by VoidX\n\"http://en.sbence.hu/\""), "About");
    }
}
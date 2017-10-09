using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DocSharp {
    public partial class DocSharp : Form {
        TreeNode import;

        void LoadRecent(object sender, EventArgs e) {
            LoadFrom(((ToolStripMenuItem)sender).Text);
        }

        void LoadRecents() {
            loadRecentToolStripMenuItem.DropDownItems.Clear();
            string[] recents = Properties.Settings.Default.Recents.Split('\n');
            for (int i = 0; i < recents.Length - 1; ++i) {
                ToolStripItem recent = new ToolStripMenuItem(recents[i]);
                recent.Click += LoadRecent;
                loadRecentToolStripMenuItem.DropDownItems.Add(recent);
            }
        }

        public DocSharp() {
            InitializeComponent();
            LoadRecents();
        }

        void ParseBlock(string code, TreeNode node) {
            int codeLen = code.Length, lastEnding = 0, lastSlash = -2, parenthesis = 0;
            bool commentLine = false, summaryLine = false;
            string summary = string.Empty;

            for (int i = 0; i < codeLen; ++i) {
                switch (code[i]) {
                    case ',':
                    case ';':
                    case '{':
                    case '}':
                        if (!commentLine && parenthesis == 0) {
                            bool instruction = code[i] == ';', block = code[i] == '{', closing = code[i] == '}';
                            string cutout = code.Substring(lastEnding, i - lastEnding).Trim();
                            lastEnding = i + 1;

                            // Remove multiline comments
                            int commentBlockStart;
                            while ((commentBlockStart = cutout.IndexOf("/*")) != -1)
                                cutout = cutout.Substring(0, commentBlockStart) + cutout.Substring(cutout.IndexOf("*/", commentBlockStart + 2) + 2);

                            if (cutout.StartsWith("using"))
                                break;
                            if (cutout.EndsWith("="))
                                cutout = cutout.Remove(cutout.Length - 2, 1).TrimEnd();

                            // Attributes
                            string attributes = string.Empty;
                            while (cutout.StartsWith("[")) {
                                int endPos = cutout.IndexOf("]");
                                string between = cutout.Substring(1, endPos - 1);
                                if (attributes.Equals(string.Empty))
                                    attributes = between;
                                else
                                    attributes += ", " + between;
                                cutout = cutout.Substring(endPos + 1).TrimStart();
                            }

                            // Default value
                            string defaultValue = string.Empty;
                            int eqPos = cutout.LastIndexOf('=');
                            if (eqPos != -1 && (cutout.LastIndexOf('(', eqPos) == -1 || cutout.IndexOf(')', eqPos) == -1)) { // not a parameter
                                defaultValue = cutout.Substring(eqPos + 1).TrimStart();
                                cutout = cutout.Substring(0, eqPos).TrimEnd();
                            }

                            // Visibility
                            Visibility vis = Visibility.Default;
                            if (Utils.RemoveModifier(ref cutout, "private")) vis = Visibility.Private;
                            else if (Utils.RemoveModifier(ref cutout, "protected")) vis = Visibility.Protected;
                            else if (Utils.RemoveModifier(ref cutout, "internal")) vis = Visibility.Internal;
                            else if (Utils.RemoveModifier(ref cutout, "public")) vis = Visibility.Public;

                            // Modifiers
                            string modifiers = string.Empty;
                            Utils.MoveModifiers(ref cutout, ref modifiers, Constants.modifiers);

                            // Type
                            string type = string.Empty;
                            int spaceIndex = cutout.IndexOf(' ');
                            if (spaceIndex != -1) {
                                type = cutout.Substring(0, spaceIndex);
                                if (type.IndexOf('(') != -1)
                                    type = "Constructor";
                                else
                                    cutout = cutout.Substring(spaceIndex).TrimStart();
                            }
                            Kinds kind = Kinds.Variables;
                            if (type.Equals("class")) kind = Kinds.Classes;
                            else if (type.Equals("interface")) kind = Kinds.Interfaces;
                            else if (type.Equals("namespace")) kind = Kinds.Namespaces;
                            else if (type.Equals("enum")) kind = Kinds.Enums;
                            else if (type.Equals("struct")) kind = Kinds.Structs;

                            // Extension
                            string extends = string.Empty;
                            int extensionIndex = cutout.IndexOf(':');
                            if (extensionIndex != -1) {
                                extends = cutout.Substring(extensionIndex + 1).TrimStart();
                                cutout = cutout.Substring(0, extensionIndex).TrimEnd();
                            }

                            // Default visibility
                            if (vis == Visibility.Default && kind != Kinds.Namespaces) {
                                if (node.Tag != null && (((ObjectInfo)node.Tag).Kind == Kinds.Enums ||
                                    ((ObjectInfo)node.Tag).Kind == Kinds.Interfaces))
                                    vis = Visibility.Public;
                                else if (kind == Kinds.Classes || kind == Kinds.Interfaces || kind == Kinds.Structs)
                                    vis = Visibility.Internal;
                                else
                                    vis = Visibility.Private;
                            }

                            // Multiple ; support
                            if (instruction && cutout.Equals(string.Empty)) {
                                summary = string.Empty;
                                break;
                            }

                            // Node handling
                            TreeNode newNode = Utils.GetNodeByText(node, cutout);
                            if (!cutout.Equals(string.Empty)) {
                                if (newNode == null)
                                    Utils.MakeNodeName(newNode = node.Nodes.Add(cutout), vis);
                                if (block) {
                                    node = newNode;
                                    if (kind == Kinds.Variables)
                                        kind = cutout.IndexOf('(') != -1 ? Kinds.Functions : Kinds.Properties;
                                }
                                if (newNode.Tag != null) {
                                    ObjectInfo tag = (ObjectInfo)newNode.Tag;
                                    tag.Summary += summary;
                                    newNode.Tag = tag;
                                }
                                else
                                    newNode.Tag = new ObjectInfo {
                                        Name = cutout, Attributes = attributes, DefaultValue = defaultValue, Extends = extends,
                                        Modifiers = modifiers.Trim(), Summary = summary, Type = type, Vis = vis, Kind = kind
                                    };
                                if (type.Equals(string.Empty) && newNode.Parent.Nodes.Count > 1) // "int a, b;" case, copy tags
                                    newNode.Tag = newNode.Parent.Nodes[newNode.Parent.Nodes.Count - 2].Tag;
                            }
                            if (closing)
                                node = node.Parent;
                            summary = string.Empty;
                        }
                        break;
                    case '(':
                    case '[':
                        if (!commentLine)
                            ++parenthesis;
                        break;
                    case ')':
                    case ']':
                        if (!commentLine)
                            --parenthesis;
                        break;
                    case '/':
                        if (!commentLine) {
                            if (lastSlash == i - 1)
                                commentLine = true;
                            lastSlash = i;
                        } else if (commentLine && !summaryLine) {
                            if (lastSlash == i - 1)
                                summaryLine = true;
                            lastSlash = i;
                        }
                        break;
                    case '#':
                        commentLine = true;
                        break;
                    case '\n':
                        if (commentLine) {
                            commentLine = false;
                            lastEnding = i + 1;
                        }
                        if (summaryLine) {
                            summary += code.Substring(lastSlash + 1, i - lastSlash - 1).Trim() + '\n';
                            summaryLine = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        void LoadFrom(string path) {
            string[] files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            sourceInfo.Nodes.Clear();
            TreeNode global = import = sourceInfo.Nodes.Add(path.Substring(path.LastIndexOf('\\') + 1));
            sourceInfo.BeginUpdate();
            foreach (string file in files) {
                string text = File.ReadAllText(file);
                ParseBlock(text, global);
            }
            sourceInfo.EndUpdate();
            Utils.ClearFunctionBodies(global);
        }

        void loadSourceToolStripMenuItem_Click(object sender, EventArgs e) {
            if (folderDialog.ShowDialog() == DialogResult.OK) {
                LoadFrom(folderDialog.SelectedPath);
                if (!Properties.Settings.Default.Recents.Contains(folderDialog.SelectedPath + '\n')) {
                    string newRecents = folderDialog.SelectedPath + '\n' + Properties.Settings.Default.Recents;
                    while (newRecents.Count(c => c == '\n') > 10)
                        newRecents = newRecents.Substring(0, newRecents.LastIndexOf('\n'));
                    Properties.Settings.Default.Recents = newRecents;
                    Properties.Settings.Default.Save();
                    LoadRecents();
                }
            }
        }

        private void sourceInfo_AfterSelect(object sender, TreeViewEventArgs e) {
            string info = string.Empty;
            TreeNode node = sourceInfo.SelectedNode;
            if (node != null && node.Tag != null) {
                ObjectInfo tag = (ObjectInfo)node.Tag;
                Utils.AppendIfExists(ref info, "Attributes", tag.Attributes);
                Utils.AppendIfExists(ref info, "Visibility", tag.Vis.ToString().ToLower());
                Utils.AppendIfExists(ref info, "Modifiers", tag.Modifiers);
                Utils.AppendIfExists(ref info, "Type", tag.Type);
                Utils.AppendIfExists(ref info, "Default value", tag.DefaultValue);
                Utils.AppendIfExists(ref info, "Extends", tag.Extends);
            }
            infoLabel.Text = info;
        }

        void SetExportability(TreeNode node) {
            foreach (TreeNode child in node.Nodes) {
                ObjectInfo tag = ((ObjectInfo)child.Tag);
                if (tag.Vis == Visibility.Default || (tag.Vis == Visibility.Public && exportPublic.Checked) ||
                    (tag.Vis == Visibility.Internal && exportInternal.Checked) || (tag.Vis == Visibility.Protected && exportProtected.Checked) ||
                    (tag.Vis == Visibility.Private && exportPrivate.Checked))
                    tag.Exportable = true;
                if (tag.Exportable) {
                    child.Tag = tag;
                    if (tag.Kind == Kinds.Enums) {
                        if (expandEnums.Checked)
                            SetExportability(child);
                    } else if (tag.Kind == Kinds.Structs) {
                        if (expandStructs.Checked)
                            SetExportability(child);
                    } else
                        SetExportability(child);
                }
            }
        }

        private void generateButton_Click(object sender, EventArgs e) {
            if (import == null)
                return;

            SetExportability(import);

            if (folderDialog.ShowDialog() == DialogResult.OK) {
                DirectoryInfo dir = new DirectoryInfo(folderDialog.SelectedPath);
                if ((dir.GetDirectories().Length == 0 && dir.GetDirectories().Length == 0) ||
                    MessageBox.Show("The folder (" + folderDialog.SelectedPath + ") is not empty. Continue generation anyway?", "Warning",
                    MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    Design.Extension = extension.Text.Equals(string.Empty) ? "html" : extension.Text;
                    Design.ExportAttributes = exportAttributes.Checked;
                    Design.GenerateDocumentation(import, folderDialog.SelectedPath);
                    Process.Start(folderDialog.SelectedPath);
                    if (phpFillers.Checked && !extension.Text.Equals("php"))
                        Utils.FillWithPHP(new DirectoryInfo(folderDialog.SelectedPath));
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("http://www.voidx.tk/");
        }
    }
}

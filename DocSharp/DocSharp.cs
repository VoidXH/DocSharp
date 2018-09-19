using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DocSharp {
    public partial class DocSharp : Form {
        string lastLoaded = string.Empty;
        Font italic;
        TreeNode import;

        void LoadRecent(string recent) {
            string newRecents = Properties.Settings.Default.Recents;
            if (newRecents.Length == 0)
                Properties.Settings.Default.Recents = recent;
            else {
                int recentPos = newRecents.IndexOf(recent);
                if (recentPos == -1)
                    Properties.Settings.Default.Recents = recent + '\n' + newRecents;
                else
                    Properties.Settings.Default.Recents = recent + '\n' + newRecents.Substring(0, recentPos) +
                        newRecents.Substring(recentPos + recent.Length + 1);
            }
            Properties.Settings.Default.Save();
            LoadRecents();
            LoadFrom(recent, defines.Text);
        }

        void LoadRecent(object sender, EventArgs e) {
            LoadRecent(((ToolStripMenuItem)sender).Text);
        }

        void LoadRecents() {
            loadRecentToolStripMenuItem.DropDownItems.Clear();
            string[] recents = Properties.Settings.Default.Recents.Split('\n');
            for (int i = 0; i < recents.Length - 1; ++i) {
                ToolStripItem recent = new ToolStripMenuItem(recents[i]);
                if (Directory.Exists(recents[i]))
                    recent.Click += LoadRecent;
                else
                    recent.Enabled = false;
                loadRecentToolStripMenuItem.DropDownItems.Add(recent);
            }
        }

        public DocSharp() {
            InitializeComponent();
            italic = new Font(sourceInfo.Font, FontStyle.Italic);
            LoadRecents();
            expandEnums.Checked = Properties.Settings.Default.ExpandEnums;
            expandStructs.Checked = Properties.Settings.Default.ExpandStructs;
            exportAttributes.Checked = Properties.Settings.Default.ExportAttributes;
            extension.Text = Properties.Settings.Default.FileExtension;
            defines.Text = Properties.Settings.Default.DefineConstants;
            phpFillers.Checked = Properties.Settings.Default.PhpFillers;
            exportInternal.Checked = Properties.Settings.Default.VisibilityInternal;
            exportPrivate.Checked = Properties.Settings.Default.VisibilityPrivate;
            exportProtected.Checked = Properties.Settings.Default.VisibilityProtected;
            exportPublic.Checked = Properties.Settings.Default.VisibilityPublic;
        }

        private void DocSharp_FormClosed(object sender, FormClosedEventArgs e) {
            Properties.Settings.Default.ExpandEnums = expandEnums.Checked;
            Properties.Settings.Default.ExpandStructs = expandStructs.Checked;
            Properties.Settings.Default.ExportAttributes = exportAttributes.Checked;
            Properties.Settings.Default.FileExtension = extension.Text;
            Properties.Settings.Default.DefineConstants = defines.Text;
            Properties.Settings.Default.PhpFillers = phpFillers.Checked;
            Properties.Settings.Default.VisibilityInternal = exportInternal.Checked;
            Properties.Settings.Default.VisibilityPrivate = exportPrivate.Checked;
            Properties.Settings.Default.VisibilityProtected = exportProtected.Checked;
            Properties.Settings.Default.VisibilityPublic = exportPublic.Checked;
            Properties.Settings.Default.Save();
        }

        bool ParseSimpleIf(string parse, string[] defineConstants) {
            int bracketPos;
            while ((bracketPos = parse.LastIndexOf('(')) != -1) {
                int endBracket = parse.IndexOf(')', bracketPos + 1);
                parse = parse.Substring(0, bracketPos) +
                    (ParseSimpleIf(parse.Substring(bracketPos + 1, endBracket - bracketPos - 1), defineConstants) ? "true" : "false") +
                    parse.Substring(endBracket + 1);
            }
            string[] split = System.Text.RegularExpressions.Regex.Split(parse, @"\s+");
            int arrPos = 0, splitEnd = split.Length - 1;
            while (arrPos < splitEnd) {
                bool first = split[arrPos].Equals("true") || Utils.ArrayContains(defineConstants, split[arrPos]),
                    second = split[arrPos + 2].Equals("true") || Utils.ArrayContains(defineConstants, split[arrPos + 2]);
                split[arrPos + 2] = (split[arrPos + 1].Equals("&&") ? first && second : first || second) ? "true" : "false";
                arrPos += 2;
            }
            return split[arrPos].Equals("true");
        }

        void ParseBlock(string code, TreeNode node, string defines) {
            int codeLen = code.Length, lastEnding = 0, lastSlash = -2, parenthesis = 0, depth = 0, lastRemovableDepth = 0;
            bool commentLine = false, inRemovableBlock = false, inString = false, preprocessorLine = false, preprocessorSkip = false, summaryLine = false;
            string summary = string.Empty;

            // Parse defines
            string[] defineConstants = defines.Split(';');
            int defineCount = defineConstants.Length;
            for (int i = 0; i < defineCount; ++i)
                defineConstants[i] = defineConstants[i].Trim();

            for (int i = 0; i < codeLen; ++i) {
                if (i != 0 && code[i - 1] != '\\') {
                    // Skip strings
                    if (!inString && code[i] == '"')
                        inString = true;
                    else if (inString) {
                        if (code[i] == '"')
                            inString = false;
                        else
                            continue;
                    // Skip characters
                    } else if (code[i] == '\'') {
                        if (code[i + 3] == '\'')
                            i += 4;
                        if (code[i + 2] == '\'')
                            i += 3;
                    }
                }
                // Actual parser
                switch (code[i]) {
                    case ',':
                    case ';':
                    case '{':
                    case '}':
                        if (!commentLine && !preprocessorSkip && parenthesis == 0) {
                            bool instruction = code[i] == ';', block = code[i] == '{', closing = code[i] == '}';
                            if (block) ++depth;
                            if (closing) {
                                --depth;
                                if (inRemovableBlock && depth < lastRemovableDepth) {
                                    inRemovableBlock = false;
                                    lastEnding = i;
                                }
                            }
                            if (inRemovableBlock)
                                continue;
                            string cutout = code.Substring(lastEnding, i - lastEnding).Trim();
                            lastEnding = i + 1;

                            // Remove multiline comments
                            int commentBlockStart;
                            while ((commentBlockStart = cutout.IndexOf("/*")) != -1) {
                                int commentEnd = cutout.IndexOf("*/", commentBlockStart + 2) + 2;
                                if (commentEnd == 1)
                                    break;
                                cutout = cutout.Substring(0, commentBlockStart) + cutout.Substring(commentEnd);
                            }

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
                            int eqPos = -1;
                            while ((eqPos = cutout.IndexOf('=', eqPos + 1)) != -1) {
                                if ((cutout.LastIndexOf('(', eqPos) == -1 || cutout.IndexOf(')', eqPos) == -1)) { // not a parameter
                                    defaultValue = cutout.Substring(eqPos + 1).TrimStart();
                                    cutout = cutout.Substring(0, eqPos).TrimEnd();
                                    break;
                                }
                            }

                            // Visibility
                            Visibility vis = Visibility.Default;
                            if (Utils.RemoveModifier(ref cutout, "private")) vis = Visibility.Private;
                            else if (Utils.RemoveModifier(ref cutout, "protected")) vis = Visibility.Protected;
                            else if (Utils.RemoveModifier(ref cutout, "internal")) vis = Visibility.Internal;
                            else if (Utils.RemoveModifier(ref cutout, "public")) vis = Visibility.Public;

                            // Modifiers
                            string modifiers = string.Empty;
                            Utils.RemoveModifier(ref cutout, "partial ");
                            Utils.MoveModifiers(ref cutout, ref modifiers, Constants.modifiers);

                            // Type
                            string type = string.Empty;
                            int spaceIndex = -1;
                            while ((spaceIndex = cutout.IndexOf(' ', spaceIndex + 1)) != -1) {
                                if ((spaceIndex == -1 || !cutout.Substring(0, spaceIndex).Equals("delegate")) && // not just the delegate word
                                    (cutout.LastIndexOf('<', spaceIndex) == -1 || cutout.IndexOf('>', spaceIndex) == -1)) { // not in a template type
                                    type = cutout.Substring(0, spaceIndex);
                                    if (type.IndexOf('(') != -1)
                                        type = "Constructor";
                                    else
                                        cutout = cutout.Substring(spaceIndex).TrimStart();
                                    break;
                                }
                            }
                            Element kind = Element.Variables;
                            if (type.Equals("class")) kind = Element.Classes;
                            else if (type.Equals("interface")) kind = Element.Interfaces;
                            else if (type.Equals("namespace")) kind = Element.Namespaces;
                            else if (type.Equals("enum")) kind = Element.Enums;
                            else if (type.Equals("struct")) kind = Element.Structs;

                            // Extension
                            string extends = string.Empty;
                            int extensionIndex = cutout.IndexOf(':');
                            if (extensionIndex != -1) {
                                extends = cutout.Substring(extensionIndex + 1).TrimStart();
                                cutout = cutout.Substring(0, extensionIndex).TrimEnd();
                            }

                            // Default visibility
                            if (vis == Visibility.Default && kind != Element.Namespaces) {
                                if (node.Tag != null && (((ElementInfo)node.Tag).Kind == Element.Enums ||
                                    ((ElementInfo)node.Tag).Kind == Element.Interfaces))
                                    vis = Visibility.Public;
                                else if (kind == Element.Classes || kind == Element.Interfaces || kind == Element.Structs)
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
                                if (newNode == null) {
                                    Utils.MakeNodeName(newNode = node.Nodes.Add(Utils.RemoveParamNames(cutout)), vis);
                                    if (modifiers.Contains("abstract"))
                                        newNode.NodeFont = italic;
                                }
                                if (block) {
                                    node = newNode;
                                    if (kind == Element.Variables) {
                                        kind = cutout.IndexOf('(') != -1 ? Element.Functions : Element.Properties;
                                        lastRemovableDepth = depth;
                                        inRemovableBlock = true;
                                    }
                                }
                                if (newNode.Tag != null) {
                                    ElementInfo tag = (ElementInfo)newNode.Tag;
                                    tag.Summary += summary;
                                    newNode.Tag = tag;
                                } else if (type.Equals(string.Empty) && newNode.Parent.Nodes.Count > 1) { // "int a, b;" case, copy tags
                                    ElementInfo inherited = (ElementInfo)newNode.Parent.Nodes[newNode.Parent.Nodes.Count - 2].Tag;
                                    inherited.Name = cutout;
                                    inherited.Summary = summary;
                                    newNode.Tag = inherited;
                                } else
                                    newNode.Tag = new ElementInfo {
                                        Name = cutout, Attributes = attributes, DefaultValue = defaultValue, Extends = extends,
                                        Modifiers = modifiers.Trim(), Summary = summary, Type = type, Vis = vis, Kind = kind
                                    };
                            }
                            if (closing)
                                node = node.Parent;
                            summary = string.Empty;
                        }
                        break;
                    case '(':
                    case '[':
                    case '<':
                        if (!inRemovableBlock && !commentLine && !preprocessorSkip)
                            ++parenthesis;
                        break;
                    case ')':
                    case ']':
                    case '>':
                        if (!inRemovableBlock && !commentLine && !preprocessorSkip)
                            --parenthesis;
                        break;
                    case '/':
                        if (!preprocessorSkip) {
                            if (!commentLine) {
                                if (lastSlash == i - 1)
                                    commentLine = true;
                                lastSlash = i;
                            } else if (commentLine && !summaryLine) {
                                if (lastSlash == i - 1)
                                    summaryLine = true;
                                lastSlash = i;
                            }
                        }
                        break;
                    case '#':
                        commentLine = preprocessorLine = true;
                        break;
                    case '\n':
                        if (preprocessorLine) {
                            string line = code.Substring(lastEnding, i - lastEnding).Trim();
                            if (line.StartsWith("#define")) {
                                string defined = line.Substring(line.IndexOf(' ')).TrimStart();
                                Array.Resize(ref defineConstants, ++defineCount);
                                defineConstants[defineCount - 1] = defined;
                            } else if (line.StartsWith("#if") || line.StartsWith("#elif")) {
                                int commentPos;
                                if ((commentPos = line.IndexOf("//")) != -1)
                                    line = line.Substring(0, commentPos).TrimEnd();
                                preprocessorSkip = !ParseSimpleIf(line.Substring(line.IndexOf(' ')).TrimStart(), defineConstants);
                            } else if (line.StartsWith("#endif"))
                                preprocessorSkip = false;
                            preprocessorLine = false;
                        }
                        if (commentLine || preprocessorSkip) {
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

        void LoadFrom(string path, string defines) {
            if (!Directory.Exists(path))
                return;
            lastLoaded = path;
            currentDefines.Text = "Code loaded with: " + (defines.Equals(string.Empty) ? "-" : defines);
            string[] files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            sourceInfo.Nodes.Clear();
            TreeNode global = import = sourceInfo.Nodes.Add(path.Substring(path.LastIndexOf('\\') + 1));
            sourceInfo.BeginUpdate();
            foreach (string file in files) {
                string text = File.ReadAllText(file);
                ParseBlock(text, global, defines);
            }
            sourceInfo.EndUpdate();
        }

        private void ReloadConstants_Click(object sender, EventArgs e) {
            LoadFrom(lastLoaded, defines.Text);
        }

        void LoadSourceToolStripMenuItem_Click(object sender, EventArgs e) {
            if (folderDialog.ShowDialog() == DialogResult.OK) {
                LoadFrom(folderDialog.SelectedPath, defines.Text);
                if (Properties.Settings.Default.Recents.Contains(folderDialog.SelectedPath + '\n'))
                    LoadRecent(folderDialog.SelectedPath);
                else {
                    string newRecents = folderDialog.SelectedPath + '\n' + Properties.Settings.Default.Recents;
                    while (newRecents.Count(c => c == '\n') > 10)
                        newRecents = newRecents.Substring(0, newRecents.LastIndexOf('\n'));
                    Properties.Settings.Default.Recents = newRecents;
                    Properties.Settings.Default.Save();
                    LoadRecents();
                }
            }
        }

        private void SourceInfo_AfterSelect(object sender, TreeViewEventArgs e) {
            string info = string.Empty;
            TreeNode node = sourceInfo.SelectedNode;
            if (node != null && node.Tag != null) {
                ElementInfo tag = (ElementInfo)node.Tag;
                Utils.AppendIfExists(ref info, "Attributes", tag.Attributes);
                Utils.AppendIfExists(ref info, "Visibility", tag.Vis.ToString().ToLower());
                Utils.AppendIfExists(ref info, "Modifiers", tag.Modifiers);
                Utils.AppendIfExists(ref info, "Type", tag.Type);
                Utils.AppendIfExists(ref info, "Default value", tag.DefaultValue);
                Utils.AppendIfExists(ref info, "Extends", tag.Extends);
                if (tag.Summary.Length != 0)
                    info += "\n\n" + Utils.QuickSummary(tag.Summary);
            }
            infoLabel.Text = info;
        }

        void SetExportability(TreeNode node) {
            foreach (TreeNode child in node.Nodes) {
                ElementInfo tag = ((ElementInfo)child.Tag);
                if (tag.Vis == Visibility.Default || (tag.Vis == Visibility.Public && exportPublic.Checked) ||
                    (tag.Vis == Visibility.Internal && exportInternal.Checked) || (tag.Vis == Visibility.Protected && exportProtected.Checked) ||
                    (tag.Vis == Visibility.Private && exportPrivate.Checked))
                    tag.Exportable = true;
                if (tag.Exportable) {
                    child.Tag = tag;
                    if (tag.Kind == Element.Enums) {
                        if (expandEnums.Checked)
                            SetExportability(child);
                    } else if (tag.Kind == Element.Structs) {
                        if (expandStructs.Checked)
                            SetExportability(child);
                    } else
                        SetExportability(child);
                }
            }
        }

        private void GenerateButton_Click(object sender, EventArgs e) {
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

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show("Doc# v" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion + " by VoidX\n" +
                "http://www.voidx.tk/", "About");
        }
    }
}
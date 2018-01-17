using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// Documentation exporter.
    /// </summary>
    public static partial class Design {
        /// <summary>
        /// Export the attributes for functions which have attributes.
        /// </summary>
        public static bool ExportAttributes;

        /// <summary>
        /// File extensions (the content will be HTML anyway).
        /// </summary>
        public static string Extension = "html";

        /// <summary>
        /// Create documentation for a single tree node.
        /// </summary>
        /// <param name="node">Position in the program tree</param>
        /// <param name="path">Path to the current tree node</param>
        /// <param name="depth">Depth in the tree</param>
        static void GenerateDocumentation(TreeNode node, string path, int depth) {
            Directory.CreateDirectory(path);
            GeneratePage(path + "\\index." + Extension, node, depth);
            foreach (TreeNode child in node.Nodes) {
                if (child.Tag == null || ((ObjectInfo)child.Tag).Exportable) {
                    if (((ObjectInfo)child.Tag).Kind >= Kinds.Functions)
                        GeneratePage(path + '\\' + child.Name + '.' + Extension, child, depth);
                    else
                        GenerateDocumentation(child, path + '\\' + child.Name, depth + 1);
                }
            }
        }

        /// <summary>
        /// Start the documentation generation.
        /// </summary>
        /// <param name="node">Program tree</param>
        /// <param name="path">Path to the documentation</param>
        public static void GenerateDocumentation(TreeNode node, string path) {
            File.WriteAllText(path + '\\' + stylesheet, style);
            GenerateDocumentation(node, path, 0);
        }

        static void BuildMenu(ref string output, TreeNode node, TreeNode child, bool locally, string path = "") {
            StringBuilder front = new StringBuilder();
            bool appendToFront = true;

            int indentLength = 0;
            TreeNode testNode = node;
            while ((testNode = testNode.Parent) != null)
                ++indentLength;

            foreach (TreeNode entry in node.Nodes) {
                if (((ObjectInfo)entry.Tag).Exportable) {
                    string entryText = !path.Equals(string.Empty) && entry.Nodes.Count != 0 ? menuElement : menuSubelement;
                    entryText = entryText.Replace(elementMarker,
                        Utils.RemoveParamNames(((ObjectInfo)entry.Tag).Name)).Replace(linkMarker, path + Utils.LocalLink(entry))
                        .Replace(indentMarker, indentLength != 0 ? "&nbsp;" + new string(' ', indentLength - 1) : string.Empty);
                    if (appendToFront) {
                        if (entry == child)
                            appendToFront = false;
                        front.Append(entryText);
                    } else
                        output += entryText;
                }
            }

            output = front.ToString() + output;
            if (node.Parent != null)
                BuildMenu(ref output, node.Parent, node, false, !locally ? "..\\" + path : path);
        }

        static string Base(TreeNode from, bool locally) {
            string menuBuild = string.Empty;
            BuildMenu(ref menuBuild, from, null, locally);
            return baseBuild.Replace(titleMarker, from.Name)
                .Replace(menuMarker, Utils.Indent(menuBuild.Trim(), Utils.SpacesBefore(baseBuild, baseBuild.IndexOf(menuMarker))));
        }

        static bool firstEntry, evenRow;

        static void BlockStart() {
            firstEntry = true;
            evenRow = false;
        }

        static void BlockAppend(StringBuilder builder, string entry, string description) {
            builder.Append(contentEntry.Replace(elementMarker, entry).Replace(contentMarker, description)
                    .Replace(subelementMarker, evenRow ? " class=\"" + evenRowClass + "\"" : string.Empty))
                    .Replace(cssMarker, firstEntry ? " class=\"" + firstColumnClass + "\"" : string.Empty);
            firstEntry = false;
            evenRow = !evenRow;
        }

        static string ContentBlock(List<TreeNode> nodes, string title) {
            if (nodes.Count == 0)
                return string.Empty;
            nodes.Sort((TreeNode a, TreeNode b) => { return a.Name.CompareTo(b.Name); });
            StringBuilder block = new StringBuilder("<h1>").Append(title).Append(@"</h1>
<table>");
            IEnumerator enumer = nodes.GetEnumerator();
            BlockStart();
            while (enumer.MoveNext()) {
                TreeNode node = (TreeNode)enumer.Current;
                StringBuilder link = new StringBuilder(((ObjectInfo)node.Tag).Type).Append(" <a href=\"").Append(Utils.LocalLink(node))
                    .Append("\">").Append(((ObjectInfo)node.Tag).Name).Append("</a>");
                BlockAppend(block, link.ToString(), Utils.QuickSummary(((ObjectInfo)node.Tag).Summary, node));
            }
            return block.Append(@"
</table>").ToString();
        }

        static string VisibilityContentBlock(List<TreeNode> nodes, string titlePostfix) {
            List<TreeNode>[] outs = new List<TreeNode>[(int)Visibility.Public * 2];
            for (int i = 0; i < outs.Length; ++i)
                outs[i] = new List<TreeNode>();

            IEnumerator enumer = nodes.GetEnumerator();
            while (enumer.MoveNext()) {
                TreeNode current = (TreeNode)enumer.Current;
                bool isStatic = ((ObjectInfo)current.Tag).Modifiers.Contains("static");
                outs[(int)((ObjectInfo)current.Tag).Vis - 1 + (isStatic ? (int)Visibility.Public : 0)].Add(current);
            }

            titlePostfix = ' ' + titlePostfix;
            StringBuilder output = new StringBuilder();
            for (int i = (int)Visibility.Public - 1; i > 0; --i) {
                output.Append(ContentBlock(outs[i], ((Visibility)(i + 1)).ToString() + titlePostfix));
                output.Append(ContentBlock(outs[i + (int)Visibility.Public], ((Visibility)(i + 1)).ToString() + " static" + titlePostfix));
            }

            return output.ToString();
        }

        static string Content(TreeNode node) {
            List<TreeNode>[] types = new List<TreeNode>[(int)Kinds.Variables + 1];
            for (int i = 0; i <= (int)Kinds.Variables; ++i)
                types[i] = new List<TreeNode>();

            IEnumerator enumer = node.Nodes.GetEnumerator();
            while (enumer.MoveNext()) {
                TreeNode current = (TreeNode)enumer.Current;
                if (((ObjectInfo)current.Tag).Exportable)
                    types[(int)((ObjectInfo)current.Tag).Kind].Add(current);
            }

            StringBuilder output = new StringBuilder("<h1>");
            if (node.Tag != null)
                output.Append(((ObjectInfo)node.Tag).Type).Append(' ').Append(((ObjectInfo)node.Tag).Name);
            else
                output.Append(node.Name);
            output.AppendLine("</h1>");

            if (node.Tag != null) {
                ObjectInfo tag = (ObjectInfo)node.Tag;
                string summary = tag.Summary;
                BlockStart();
                output.AppendLine(Utils.RemoveTag(ref summary, "summary", node.Nodes.Count != 0 ? node.Nodes[0] : node)).Append("<table>");
                if (ExportAttributes && !tag.Attributes.Equals(string.Empty)) BlockAppend(output, "Attributes", tag.Attributes);
                if (tag.Vis != Visibility.Default) BlockAppend(output, "Visibility", tag.Vis.ToString());
                if (!tag.Modifiers.Equals(string.Empty)) BlockAppend(output, "Modifiers", tag.Modifiers);
                if (!tag.Extends.Equals(string.Empty)) BlockAppend(output, "Extends", tag.Extends);
                if (!tag.DefaultValue.Equals(string.Empty)) BlockAppend(output, "Default value", tag.DefaultValue);
                string returns = Utils.RemoveTag(ref summary, "returns", node);
                if (!returns.Equals(string.Empty)) BlockAppend(output, "Returns", returns);
                output.AppendLine("</table>");

                if (summary.Contains("</param>")) {
                    output.AppendLine("<h1>Parameters</h1>").AppendLine("<table>");
                    BlockStart();
                    string[] definedParams = tag.Name.Substring(tag.Name.IndexOf('(') + 1).Split(',', ')');
                    string[] parameters;
                    while ((parameters = Utils.RemoveParam(ref summary)) != null) {
                        string paramType = string.Empty;
                        foreach (string definedParam in definedParams) {
                            if (definedParam.Contains(parameters[0])) {
                                string cut = definedParam;
                                int eqPos;
                                if ((eqPos = cut.IndexOf('=')) != -1) // remove default value
                                    cut = cut.Substring(0, eqPos).Trim();
                                if (cut.EndsWith(parameters[0])) {
                                    cut = cut.Substring(0, cut.LastIndexOf(parameters[0]));
                                    string trim = cut.Trim();
                                    if (cut.Length != trim.Length) { // if it doesn't end with a whitespace, it's another param that ends the same way
                                        paramType = cut.Trim();
                                        break;
                                    }
                                }
                            }
                        }
                        BlockAppend(output, paramType + " <b>" + parameters[0] + "</b>", parameters[1]);
                    }
                    output.AppendLine("</table>");
                }
            }

            for (int i = 0; i <= (int)Kinds.Variables; ++i) {
                output.Append(i != (int)Kinds.Namespaces ? VisibilityContentBlock(types[i], ((Kinds)i).ToString().ToLower()) :
                    ContentBlock(types[i], "Namespaces"));
            }
            return output.ToString();
        }

        static void GeneratePage(string path, TreeNode site, int depth) {
            string baseBuild = Base(site, !path.EndsWith("index." + Extension))
                .Replace(cssMarker, string.Concat(Enumerable.Repeat("..\\", depth)) + stylesheet);
            File.WriteAllText(path, baseBuild.Replace(contentMarker, Content(site)));
        }
    }
}
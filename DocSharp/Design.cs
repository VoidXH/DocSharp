﻿using System.Collections;
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

        static Exporter exporter;

        /// <summary>
        /// Create documentation for a single tree node.
        /// </summary>
        /// <param name="node">Position in the program tree</param>
        /// <param name="path">Path to the current tree node</param>
        /// <param name="depth">Depth in the tree</param>
        static void GenerateDocumentation(TreeNode node, string path, int depth) {
            Directory.CreateDirectory(path);
            GeneratePage(path + "\\index." + Extension, node, depth);
            if (exporter != null)
                exporter.Ping(node);
            foreach (TreeNode child in node.Nodes) {
                if (child.Tag == null || ((ElementInfo)child.Tag).Exportable) {
                    if (((ElementInfo)child.Tag).Kind >= Element.Functions)
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
        /// <param name="exporter">Async export handler</param>
        public static void GenerateDocumentation(TreeNode node, string path, Exporter exporter = null) {
            File.WriteAllText(path + '\\' + stylesheet, style);
            Design.exporter = exporter;
            GenerateDocumentation(node, path, 0);
        }

        /// <summary>
        /// Create the menu on the side of a documentation page.
        /// </summary>
        /// <param name="output">Generated page content</param>
        /// <param name="node">The node of the currently exported element</param>
        /// <param name="child">Call with null, internally used when the recursion generates higher layers of the menu</param>
        /// <param name="locally">The current file is to be placed in the same folder as the index</param>
        /// <param name="path">Relative file system path to the checked node from the starting node</param>
        static void BuildMenu(ref string output, TreeNode node, TreeNode child, bool locally, string path = "") {
            StringBuilder front = new StringBuilder();
            bool appendToFront = true;

            int indentLength = 0;
            TreeNode testNode = node;
            while ((testNode = testNode.Parent) != null)
                ++indentLength;

            foreach (TreeNode entry in node.Nodes) {
                if (((ElementInfo)entry.Tag).Exportable) {
                    string entryText = !path.Equals(string.Empty) && entry.Nodes.Count != 0 ? menuElement : menuSubelement;
                    entryText = entryText.Replace(elementMarker,
                        Utils.RemoveParamNames(((ElementInfo)entry.Tag).Name)).Replace(linkMarker, path + Utils.LocalLink(entry))
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

        /// <summary>
        /// Generate the bofy of an exported page.
        /// </summary>
        /// <param name="from">Target code element</param>
        /// <param name="locally">The current file is to be placed in the same folder as the index</param>
        static string Base(TreeNode from, bool locally) {
            string menuBuild = string.Empty;
            BuildMenu(ref menuBuild, from, null, locally);
            return baseBuild.Replace(titleMarker, from.Name)
                .Replace(menuMarker, Utils.Indent(menuBuild.Trim(), Utils.SpacesBefore(baseBuild, baseBuild.IndexOf(menuMarker))));
        }

        static bool firstEntry, evenRow;

        /// <summary>
        /// Start the generation of a content block.
        /// </summary>
        static void BlockStart() {
            firstEntry = true;
            evenRow = false;
        }

        /// <summary>
        /// Add a row to a content block.
        /// </summary>
        /// <param name="builder"><see cref="StringBuilder"/> of the content block</param>
        /// <param name="entry">Property or reference name</param>
        /// <param name="description">Value or description of <paramref name="entry"/></param>
        static void BlockAppend(StringBuilder builder, string entry, string description) {
            builder.Append(contentEntry.Replace(elementMarker, entry).Replace(contentMarker, description)
                    .Replace(subelementMarker, evenRow ? evenRowClassTag : string.Empty))
                    .Replace(cssMarker, firstEntry ? firstColumnClassTag : string.Empty);
            firstEntry = false;
            evenRow = !evenRow;
        }

        /// <summary>
        /// Generate a content block for all entries in a list.
        /// </summary>
        /// <param name="nodes">Code elements</param>
        /// <param name="title">Title of the content block</param>
        /// <returns></returns>
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
                StringBuilder link = new StringBuilder(((ElementInfo)node.Tag).Type).Append(" <a href=\"").Append(Utils.LocalLink(node))
                    .Append("\">").Append(((ElementInfo)node.Tag).Name).Append("</a>");
                BlockAppend(block, link.ToString(), Utils.QuickSummary(((ElementInfo)node.Tag).Summary, node));
            }
            return block.Append(@"
</table>").ToString();
        }

        /// <summary>
        /// Generate a content block for all entries in a list, grouped by visibility.
        /// </summary>
        /// <param name="nodes">Code elements</param>
        /// <param name="titlePostfix">Postfix after visibility (e.g. functions)</param>
        static string VisibilityContentBlock(List<TreeNode> nodes, string titlePostfix) {
            List<TreeNode>[] outs = new List<TreeNode>[(int)Visibility.Public * 2];
            for (int i = 0; i < outs.Length; ++i)
                outs[i] = new List<TreeNode>();

            IEnumerator enumer = nodes.GetEnumerator();
            while (enumer.MoveNext()) {
                TreeNode current = (TreeNode)enumer.Current;
                bool isStatic = ((ElementInfo)current.Tag).Modifiers.Contains(Parser._static);
                outs[(int)((ElementInfo)current.Tag).Vis - 1 + (isStatic ? (int)Visibility.Public : 0)].Add(current);
            }

            titlePostfix = ' ' + titlePostfix;
            StringBuilder output = new StringBuilder();
            for (int i = (int)Visibility.Public - 1; i > 0; --i) {
                output.Append(ContentBlock(outs[i], ((Visibility)(i + 1)).ToString() + titlePostfix));
                output.Append(ContentBlock(outs[i + (int)Visibility.Public],
                    string.Format("{0} static{1}", ((Visibility)(i + 1)).ToString(), titlePostfix)));
            }

            return output.ToString();
        }

        /// <summary>
        /// Generate the documentation page's relevant information of the code element.
        /// </summary>
        static string Content(TreeNode node) {
            List<TreeNode>[] types = new List<TreeNode>[(int)Element.Variables + 1];
            for (int i = 0; i <= (int)Element.Variables; ++i)
                types[i] = new List<TreeNode>();

            IEnumerator enumer = node.Nodes.GetEnumerator();
            while (enumer.MoveNext()) {
                TreeNode current = (TreeNode)enumer.Current;
                if (((ElementInfo)current.Tag).Exportable)
                    types[(int)((ElementInfo)current.Tag).Kind].Add(current);
            }

            StringBuilder output = new StringBuilder("<h1>");
            if (node.Tag != null)
                output.Append(((ElementInfo)node.Tag).Type).Append(' ').Append(((ElementInfo)node.Tag).Name);
            else
                output.Append(node.Name);
            output.AppendLine("</h1>");

            if (node.Tag != null) {
                ElementInfo tag = (ElementInfo)node.Tag;
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
                                    // if it doesn't end with a whitespace, it's another param that ends the same way
                                    if (cut.Length != trim.Length) {
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

            for (int i = 0; i <= (int)Element.Variables; ++i) {
                output.Append(i != (int)Element.Namespaces ? VisibilityContentBlock(types[i], ((Element)i).ToString().ToLower()) :
                    ContentBlock(types[i], "Namespaces"));
            }
            return output.ToString();
        }

        /// <summary>
        /// Export a documentation page.
        /// </summary>
        /// <param name="path">Target file name</param>
        /// <param name="site">Node of the element to export</param>
        /// <param name="depth">Depth in the file system structure relative to the export root</param>
        static void GeneratePage(string path, TreeNode site, int depth) {
            string baseBuild = Base(site, !path.EndsWith("index." + Extension))
                .Replace(cssMarker, string.Concat(Enumerable.Repeat("..\\", depth)) + stylesheet);
            File.WriteAllText(path, baseBuild.Replace(contentMarker, Content(site)));
        }
    }
}
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// Code element kinds.
    /// </summary>
    enum Element { Namespaces, Classes, Interfaces, Enums, Structs, Functions, Properties, Variables }
    /// <summary>
    /// Code element visibilities.
    /// </summary>
    enum Visibility { Default, Private, Protected, Internal, Public }

    /// <summary>
    /// All documentation information about a code element.
    /// </summary>
    struct ElementInfo {
        public bool Exportable;
        public string Name, Attributes, DefaultValue, Extends, Modifiers, Summary, Type;
        public Visibility Vis;
        public Element Kind;
    }

    static class Constants {
        /// <summary>
        /// Element modifiers.
        /// </summary>
        public static readonly string[] modifiers = {
            "abstract ", "async ", "const ", "event ", "extern ", "new ", "override ",
            "readonly ", "sealed ", "static ", "unsafe ", "virtual ", "volatile "
        };

        /// <summary>
        /// Visibility marker characters. Their position in the array must match their position in <see cref="Visibility"/>.
        /// </summary>
        public static readonly char[] visibilities = { 'x', '-', '#', '~', '+' };
    }

    /// <summary>
    /// General utilities.
    /// </summary>
    static class Utils {
        /// <summary>
        /// If the value of a property is filled, add it in a new line to the target with a prefix.
        /// </summary>
        /// <param name="target">Output</param>
        /// <param name="name">Prefix separated with ": "</param>
        /// <param name="value">Value of the parameter called <paramref name="name"/></param>
        public static void AppendIfExists(ref string target, string name, string value) {
            if (value.Length != 0) {
                StringBuilder pass = new StringBuilder();
                if (target.Length != 0)
                    pass.Append('\n');
                pass.Append(name).Append(": ").Append(value);
                target += pass.ToString();
            }
        }

        /// <summary>
        /// Gets if an array contains a given value.
        /// </summary>
        public static bool ArrayContains<T>(T[] array, T value) {
            int len = array.Length;
            for (int i = 0; i < len; ++i)
                if (array[i].Equals(value))
                    return true;
            return false;
        }

        /// <summary>
        /// Fills a folder and all its subfolders with empty index.php files where there is no index.php found to prevent directory listing.
        /// </summary>
        public static void FillWithPHP(DirectoryInfo target) {
            string targetName = target.FullName + "\\index.php";
            if (!File.Exists(targetName))
                File.Create(targetName).Close();
            DirectoryInfo[] subdirs = target.GetDirectories();
            int subdirCount = subdirs.Length;
            for (int i = 0; i < subdirCount; ++i)
                FillWithPHP(subdirs[i]);
        }

        /// <summary>
        /// Get a child node of a <see cref="TreeNode"/> by name.
        /// </summary>
        public static TreeNode GetNodeByText(TreeNode parent, string text) {
            IEnumerator enumer = parent.Nodes.GetEnumerator();
            while (enumer.MoveNext())
                if (((TreeNode)enumer.Current).Name.Equals(text))
                    return (TreeNode)enumer.Current;
            return null;
        }

        /// <summary>
        /// Generate an indented line of code.
        /// </summary>
        /// <param name="text">Text to indent</param>
        /// <param name="chars">Amount of spaces used in indention</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Indent(string text, int chars) {
            return text.Replace("\n", '\n' + new string(' ', chars));
        }

        /// <summary>
        /// Generate link to a code element found in the same depth.
        /// </summary>
        /// <param name="to">Target code element</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string LocalLink(TreeNode to) {
            return new StringBuilder(to.Name).Append(((ElementInfo)to.Tag).Kind < Element.Functions ? "\\index." : ".")
                .Append(Design.Extension).ToString();
        }

        /// <summary>
        /// Assembles the internally used name and filename for an element.
        /// </summary>
        /// <param name="name">Name of the node (for functions, the function name only)</param>
        /// <param name="tryCount">The number of times this filename has been tried</param>
        static string NodeName(string name, int tryCount) {
            StringBuilder result = new StringBuilder(name);
            result.Replace("*", "Mul").Replace("/", "Div").Replace("<", "Lt").Replace(">", "Gt");
            if (tryCount != 0)
                result.Append(tryCount);
            return result.ToString();
        }

        /// <summary>
        /// Generates and sets the internally used name and filename for an element.
        /// </summary>
        /// <param name="node">Code element</param>
        /// <param name="vis">Visibility of the code element - required as the code element is under construction when this is called</param>
        public static void MakeNodeName(TreeNode node, Visibility vis) {
            if (node.Name.Equals(string.Empty)) {
                int parenthesis = node.Text.IndexOf('(');
                string nameOnly = parenthesis == -1 ? node.Text : node.Text.Substring(0, parenthesis);
                int angleBracket = nameOnly.IndexOf('<');
                if (angleBracket != -1)
                    nameOnly = nameOnly.Substring(0, angleBracket);
                int tryCount = 0;
                while (true) {
                    string tryWith = NodeName(nameOnly, tryCount++);
                    bool found = false;
                    foreach (TreeNode other in node.Parent.Nodes) {
                        if (other.Name.Equals(tryWith, StringComparison.OrdinalIgnoreCase)) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        node.Name = tryWith;
                        break;
                    }
                }
            }
            if (vis != Visibility.Default)
                node.Text = Constants.visibilities[(int)vis] + node.Text;
        }

        /// <summary>
        /// Move modifiers from one string to another.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveModifiers(ref string source, ref string target, params string[] modifiers) {
            IEnumerator enumer = modifiers.GetEnumerator();
            while (enumer.MoveNext()) {
                if (source.Contains((string)enumer.Current)) {
                    source = source.Replace((string)enumer.Current, string.Empty);
                    target += (string)enumer.Current;
                }
            }
            source.TrimStart();
        }

        /// <summary>
        /// Export just the summary of a code element without references.
        /// </summary>
        /// <param name="fullSummary">The entire XML block of the documentation</param>
        public static string QuickSummary(string fullSummary) {
            int summaryStart = fullSummary.IndexOf("<summary>") + 9;
            return summaryStart != 8 ? Regex.Replace(fullSummary.Substring(summaryStart, fullSummary.IndexOf("</summary>") - summaryStart)
                .Replace("<see cref=\"", string.Empty).Replace("\"/>", ""), @"\r\n?|\n", " ") : string.Empty;
        }

        /// <summary>
        /// Remove and export just the summary of a code element with referenes.
        /// </summary>
        /// <param name="source">The entire XML block of the documentation</param>
        /// <param name="node">Code element</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string QuickSummary(string source, TreeNode node) {
            return RemoveTag(ref source, "summary", node);
        }

        /// <summary>
        /// Remove a modifier from a signature.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveModifier(ref string source, string modifier) {
            if (source.Contains(modifier)) {
                source = source.Replace(modifier, string.Empty).TrimStart();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove the next parameter summary and return its name and description or null if there is no remaining parameter summaries.
        /// </summary>
        /// <param name="source">The entire XML block of the documentation.</param>
        public static string[] RemoveParam(ref string source) {
            int startPos = source.IndexOf("<param"); if (startPos == -1) return null;
            int cutPos = source.IndexOf('>', startPos); if (cutPos == -1) return null;
            int endPos = source.IndexOf("</param>", cutPos); if (endPos == -1) return null;
            int namePos = source.IndexOf("name", startPos); if (namePos == -1) return null;
            int nameStart = source.IndexOf('"', namePos) + 1; if (nameStart == -1) return null;
            int nameEnd = source.IndexOf('"', nameStart); if (nameEnd == -1) return null;
            string[] output = new string[2] {
                source.Substring(nameStart, nameEnd - nameStart),
                source.Substring(cutPos + 1, endPos - cutPos - 1)
            };
            source = source.Substring(0, startPos).TrimEnd() + source.Substring(endPos + 8).TrimStart();
            return output;
        }

        /// <summary>
        /// Remove parameter names and default values from a function signature, keep only their types.
        /// </summary>
        /// <param name="signature">Function signature</param>
        public static string RemoveParamNames(string signature) {
            int paramStart, paramEnd;
            if ((paramStart = signature.IndexOf('(') + 1) != 0 && (paramEnd = signature.IndexOf(')', paramStart)) != -1) {
                string parameters = signature.Substring(paramStart, paramEnd - paramStart);
                string paramTypes = string.Empty;
                string[] fullParams = parameters.Split(',');
                int paramCount = fullParams.Length;
                for (int param = 0; param < paramCount; ++param) {
                    fullParams[param] = fullParams[param].Trim();
                    if (fullParams[param].Contains('='))
                        fullParams[param] = fullParams[param].Substring(0, fullParams[param].IndexOf('=')).TrimEnd();
                    paramTypes += (fullParams[param].Contains(' ') ? fullParams[param].Substring(0, fullParams[param].LastIndexOf(' ')) :
                        fullParams[param]) + ", ";
                }
                if (paramTypes.Length != 0)
                    signature = signature.Substring(0, paramStart) + paramTypes.Substring(0, paramTypes.Length - 2)
                        + signature.Substring(paramEnd);
            }
            return signature;
        }

        /// <summary>
        /// Remove a tag from a documentation block and return its value.
        /// </summary>
        /// <param name="source">The entire XML documentation block</param>
        /// <param name="tag">Tag name</param>
        /// <param name="node">Code element</param>
        /// <returns></returns>
        public static string RemoveTag(ref string source, string tag, TreeNode node) {
            int startPos = source.IndexOf('<' + tag), endPos = source.IndexOf("</" + tag);
            if (startPos == -1 || endPos == -1)
                return string.Empty;
            int tagLength = tag.Length + 2;
            string output = source.Substring(startPos + tagLength, endPos - startPos - tagLength).Trim();
            source = source.Substring(0, startPos) + source.Substring(endPos + tagLength + 1);
            ReplaceReferences(ref output, node);
            if (output.Contains("\n"))
                output = Regex.Replace(output, @"\r\n?|\n", "<br />");
            return output;
        }

        /// <summary>
        /// Replace references in a description with links.
        /// </summary>
        /// <param name="source">Description</param>
        /// <param name="node">Code element</param>
        static void ReplaceReferences(ref string source, TreeNode node) {
            int seePos;
            while ((seePos = source.IndexOf("<see")) != -1) {
                int refStart = source.IndexOf('"', seePos); if (refStart == -1) return;
                int refEnd = source.IndexOf('"', refStart + 1); if (refEnd == -1) return;
                int seeEnd = source.IndexOf('>', seePos); if (seeEnd == -1) return;
                string reference = source.Substring(refStart + 1, refEnd - refStart - 1);
                bool linked = false, firstRun = true;
                TreeNode lookingFor = node;
                string path = string.Empty;
                while (!linked && lookingFor != null) {
                    IEnumerator children = lookingFor.Nodes.GetEnumerator();
                    while (children.MoveNext()) {
                        if (((TreeNode)children.Current).Name.Equals(reference, StringComparison.OrdinalIgnoreCase)) {
                            reference = "<a href=\"" + path + LocalLink((TreeNode)children.Current) + "\">" + reference + "</a>";
                            linked = true;
                            break;
                        }
                    }
                    lookingFor = lookingFor.Parent;
                    if (firstRun)
                        firstRun = false;
                    else
                        path += "..\\";
                }
                source = source.Substring(0, seePos) + reference + source.Substring(seeEnd + 1);
            }
        }

        /// <summary>
        /// Count the spaces before a position in the given string.
        /// </summary>
        public static int SpacesBefore(string text, int index) {
            int count = 0;
            for (int i = index - 1; i >= 0 && text[index] == ' '; --i)
                ++count;
            return count;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
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

    static class Constants {
        /// <summary>
        /// Element modifiers.
        /// </summary>
        public static readonly string[] modifiers = {
            "abstract ", "async ", "const ", "event ", "extern ", "new ", "override ",
            "readonly ", "sealed ", "static ", "unsafe ", "virtual ", "volatile "
        };

        /// <summary>
        /// Visibility marker characters. Their position in the array must match their position in
        /// <see cref="Visibility"/>.
        /// </summary>
        public static readonly char[] visibilities = { 'x', '-', '#', '~', '+' };
    }

    /// <summary>
    /// General utilities.
    /// </summary>
    static partial class Utils {
        /// <summary>
        /// If the value of a property is filled, add it in a new line to the target with a prefix.
        /// </summary>
        /// <param name="target">Output</param>
        /// <param name="name">Prefix separated with ": "</param>
        /// <param name="value">Value of the parameter called <paramref name="name"/></param>
        public static void AppendIfExists(StringBuilder target, string name, string value) {
            if (value.Length != 0) {
                if (target.Length != 0) {
                    target.AppendLine();
                }
                target.Append(name).Append(": ").Append(value);
            }
        }

        /// <summary>
        /// Gets if an array contains a given value.
        /// </summary>
        public static bool ArrayContains<T>(T[] array, T value) {
            int len = array.Length;
            for (int i = 0; i < len; i++) {
                if (array[i].Equals(value)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find the first node in the project which matches a <paramref name="predicate"/>.
        /// </summary>
        public static MemberNode FindFirst(MemberNode source, Func<MemberNode, bool> predicate) {
            while (source.Parent != null) {
                source = (MemberNode)source.Parent;
            }

            MemberNode Proc(MemberNode source) {
                foreach (MemberNode child in source.Nodes) {
                    if (predicate(child)) {
                        return child;
                    }

                    MemberNode result = Proc(child);
                    if (result != null) {
                        return result;
                    }
                }
                return null;
            }

            return Proc(source);
        }

        /// <summary>
        /// Fills a folder and all its subfolders with empty index.php files where there
        /// is no index.php found to prevent directory listing.
        /// </summary>
        public static void FillWithPHP(DirectoryInfo target) {
            string targetName = target.FullName + "\\index.php";
            if (!File.Exists(targetName)) {
                File.Create(targetName).Close();
            }
            DirectoryInfo[] subdirs = target.GetDirectories();
            int subdirCount = subdirs.Length;
            for (int i = 0; i < subdirCount; i++)
                FillWithPHP(subdirs[i]);
        }

        /// <summary>
        /// Get the fully qualified name of a node.
        /// </summary>
        public static string FullyQualifiedName(MemberNode node) {
            List<string> chain = new();
            while (node != null) {
                if (node.name != null) {
                    chain.Add(HttpUtility.HtmlEncode(node.name));
                }
                node = (MemberNode)node.Parent;
            }
            chain.Reverse();
            return string.Join(".", chain.ToArray());
        }

        /// <summary>
        /// Get a child node of a <see cref="TreeNode"/> by name.
        /// </summary>
        public static MemberNode GetNodeByText(MemberNode parent, string text) {
            IEnumerator enumer = parent.Nodes.GetEnumerator();
            while (enumer.MoveNext()) {
                if (((MemberNode)enumer.Current).Name.Equals(text)) {
                    return (MemberNode)enumer.Current;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a tag's HTML output with the links replaced and the references assigned.
        /// </summary>
        /// <param name="source">The entire XML documentation block</param>
        /// <param name="tag">Tag name</param>
        /// <param name="node">Code element</param>
        public static string GetTag(string source, string tag, MemberNode node) {
            int startPos = source.IndexOf('<' + tag), endPos = source.IndexOf("</" + tag);
            if (startPos == -1 || endPos == -1) {
                return string.Empty;
            }
            int tagLength = tag.Length + 2;
            string output = source.Substring(startPos + tagLength, endPos - startPos - tagLength).Trim();
            ReplaceReferences(ref output, node);
            if (output.Contains('\n')) {
                output = newLineToBr().Replace(output, "<br />");
            }
            return output;
        }

        /// <summary>
        /// Generate an indented line of code.
        /// </summary>
        /// <param name="text">Text to indent</param>
        /// <param name="chars">Amount of spaces used in indention</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Indent(string text, int chars) =>
            text.Replace("\n", '\n' + new string(' ', chars));

        /// <summary>
        /// Finds the base class which was inherited, and get its entire XML documentation.
        /// </summary>
        public static void InheritDocumentation(MemberNode node) {
            MemberNode parent = FindFirst(node, parent => parent.Name.Equals(node.Name) && parent.summary.Contains("<summary>"));
            if (parent != null) {
                node.summary = parent.summary;
            }
        }

        /// <summary>
        /// Generate link to a code element found in the same depth.
        /// </summary>
        /// <param name="to">Target code element</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string LocalLink(MemberNode to) =>
            new StringBuilder(to.Name).Append(to.Kind < Element.Functions ? "\\index." : ".").Append(Design.extension).ToString();

        /// <summary>
        /// Assembles the internally used name and filename for an element.
        /// </summary>
        /// <param name="name">Name of the node (for functions, the function name only)</param>
        /// <param name="tryCount">The number of times this filename has been tried</param>
        static string NodeName(string name, int tryCount) {
            StringBuilder result = new(name);
            result.Replace("*", "Mul").Replace("/", "Div").Replace("<", "Lt").Replace(">", "Gt");
            if (tryCount != 0) {
                result.Append(tryCount);
            }
            return result.ToString();
        }

        /// <summary>
        /// Generates and sets the internally used name and filename for an element.
        /// </summary>
        /// <param name="node">Code element</param>
        public static void MakeNodeName(MemberNode node) {
            if (node.Name.Equals(string.Empty)) {
                int parenthesis = node.Text.IndexOf('(');
                string nameOnly = parenthesis == -1 ? node.Text : node.Text[..parenthesis];
                int angleBracket = nameOnly.IndexOf('<');
                if (angleBracket != -1) {
                    nameOnly = nameOnly[..angleBracket];
                }
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
            if (node.Vis != Visibility.Default) {
                node.Text = Constants.visibilities[(int)node.Vis] + node.Text;
            }
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
            source = source.TrimStart();
        }

        /// <summary>
        /// Export just the summary of a code element without references.
        /// </summary>
        /// <param name="fullSummary">The entire XML block of the documentation</param>
        public static string QuickSummary(string fullSummary) {
            int summaryStart = fullSummary.IndexOf("<summary>") + 9,
                summaryLength = fullSummary.IndexOf("</summary>") - summaryStart;
            return summaryStart != 8 && summaryLength > 0 ? newLineToBr().Replace(fullSummary.Substring(summaryStart, summaryLength)
                .Replace("<see cref=\"", string.Empty).Replace("\"/>", ""), " ").Trim() : string.Empty;
        }

        /// <summary>
        /// Remove and export just the summary of a code element with referenes.
        /// </summary>
        /// <param name="source">The entire XML block of the documentation</param>
        /// <param name="node">Code element</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string QuickSummary(string source, MemberNode node) => RemoveTag(ref source, "summary", node).Trim();

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
        /// Removes all occurences of a HTML tag.
        /// </summary>
        public static string RemoveHTMLTag(string source, string tag) {
            while (true) {
                int index = source.IndexOf('<' + tag);
                if (index == -1) {
                    break;
                }
                int index2 = source.IndexOf('>', index + tag.Length);
                if (index2 == -1) {
                    break;
                }
                source = source[..index] + source[(index2 + 1)..];
            }
            return tag[0] != '/' ? RemoveHTMLTag(source, '/' + tag) : source;
        }

        /// <summary>
        /// Remove the links from a HTML code.
        /// </summary>
        public static string RemoveLinks(string source) => RemoveHTMLTag(source, "a");

        /// <summary>
        /// Remove the next parameter summary and return its name and description or null if there
        /// is no remaining parameter summaries.
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
                source[nameStart..nameEnd ],
                source.Substring(cutPos + 1, endPos - cutPos - 1)
            };
            source = source[..startPos].TrimEnd() + source[(endPos + 8)..].TrimStart();
            return output;
        }

        /// <summary>
        /// Remove parameter names and default values from a function signature, keep only their types.
        /// </summary>
        /// <param name="signature">Function signature</param>
        public static string RemoveParamNames(string signature) {
            int paramStart, paramEnd;
            if ((paramStart = signature.IndexOf('(') + 1) != 0 && (paramEnd = signature.IndexOf(')', paramStart)) != -1) {
                string parameters = signature[paramStart..paramEnd];
                string paramTypes = string.Empty;
                string[] fullParams = parameters.Split(',');
                int paramCount = fullParams.Length;
                for (int param = 0; param < paramCount; param++) {
                    fullParams[param] = fullParams[param].Trim();
                    if (fullParams[param].Contains('=')) {
                        fullParams[param] = fullParams[param][..fullParams[param].IndexOf('=')].TrimEnd();
                    }
                    paramTypes += (fullParams[param].Contains(' ') ?
                        fullParams[param][..fullParams[param].LastIndexOf(' ')] :
                        fullParams[param]) + ", ";
                }
                if (paramTypes.Length != 0) {
                    signature = signature[..paramStart] + paramTypes[..^2] + signature[paramEnd..];
                }
            }
            return signature;
        }

        /// <summary>
        /// Remove a tag from a documentation block and return its value.
        /// </summary>
        /// <param name="source">The entire XML documentation block</param>
        /// <param name="tag">Tag name</param>
        /// <param name="node">Code element</param>
        public static string RemoveTag(ref string source, string tag, MemberNode node) {
            int startPos = source.IndexOf('<' + tag), endPos = source.IndexOf("</" + tag);
            if (startPos == -1 || endPos == -1)
                return string.Empty;
            int tagLength = tag.Length + 2;
            string output = source.Substring(startPos + tagLength, endPos - startPos - tagLength).Trim();
            source = source[..startPos] + source[(endPos + tagLength + 1)..];
            ReplaceReferences(ref output, node);
            if (output.Contains('\n')) {
                output = newLineToBr().Replace(output, "<br />");
            }
            return output;
        }

        /// <summary>
        /// Replace references in a description with links.
        /// </summary>
        /// <param name="source">Description</param>
        /// <param name="node">Code element</param>
        static void ReplaceReferences(ref string source, MemberNode node) {
            int seePos;
            while ((seePos = source.IndexOf("<see")) != -1) {
                int refStart = source.IndexOf('"', seePos); if (refStart == -1) return;
                int refEnd = source.IndexOf('"', refStart + 1); if (refEnd == -1) return;
                int seeEnd = source.IndexOf('>', seePos); if (seeEnd == -1) return;
                string reference = source.Substring(refStart + 1, refEnd - refStart - 1);
                bool linked = false, firstRun = true;
                MemberNode lookingFor = node;
                string path = string.Empty;
                while (!linked && lookingFor != null) {
                    IEnumerator children = lookingFor.Nodes.GetEnumerator();
                    while (children.MoveNext()) {
                        MemberNode child = (MemberNode)children.Current;
                        if (child.Name.Equals(reference, StringComparison.OrdinalIgnoreCase)) {
                            reference = "<a href=\"" + path + LocalLink(child) + "\">" + reference + "</a>";
                            child.export.referencedBy.Add(node);
                            linked = true;
                            break;
                        }
                    }
                    lookingFor = (MemberNode)lookingFor.Parent;
                    if (firstRun) {
                        firstRun = false;
                    } else {
                        path += "..\\";
                    }
                }
                source = source[..seePos] + reference + source[(seeEnd + 1)..];
            }
        }

        /// <summary>
        /// Count the spaces before a position in the given string.
        /// </summary>
        public static int SpacesBefore(string text, int index) {
            int count = 0;
            for (int i = index - 1; i >= 0 && text[index] == ' '; i--) {
                count++;
            }
            return count;
        }

        [GeneratedRegex("\\r\\n?|\\n")]
        private static partial Regex newLineToBr();
    }
}
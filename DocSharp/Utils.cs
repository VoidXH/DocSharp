using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DocSharp {
    enum Kinds { Namespaces, Classes, Interfaces, Enums, Structs, Functions, Properties, Variables }
    enum Visibility { Default, Private, Protected, Internal, Public }

    struct ObjectInfo {
        public bool Exportable;
        public string Name, Attributes, DefaultValue, Extends, Modifiers, Summary, Type;
        public Visibility Vis;
        public Kinds Kind;
    }

    static class Constants {
        public static readonly string[] modifiers = {
            "abstract ", "async ", "const ", "event ", "extern ", "new ", "override ",
            "readonly ", "sealed ", "static ", "unsafe ", "virtual ", "volatile "
        };

        public static readonly char[] visibilities = { 'x', '-', '#', '~', '+' };
    }

    static class Utils {
        public static void AppendIfExists(ref string target, string name, string value) {
            if (!value.Equals(string.Empty)) {
                string pass = name + ": " + value;
                if (target.Equals(string.Empty))
                    target = pass;
                else
                    target += '\n' + pass;
            }
        }

        public static bool ArrayContains<T>(T[] array, T value) {
            int len = array.Length;
            for (int i = 0; i < len; ++i)
                if (array[i].Equals(value))
                    return true;
            return false;
        }

        public static void FillWithPHP(DirectoryInfo target) {
            string targetName = target.FullName + "\\index.php";
            if (!File.Exists(targetName))
                File.Create(targetName).Close();
            DirectoryInfo[] subdirs = target.GetDirectories();
            int subdirCount = subdirs.Length;
            for (int i = 0; i < subdirCount; ++i)
                FillWithPHP(subdirs[i]);
        }

        public static TreeNode GetNodeByText(TreeNode parent, string text) {
            IEnumerator enumer = parent.Nodes.GetEnumerator();
            while (enumer.MoveNext())
                if (((TreeNode)enumer.Current).Name.Equals(text))
                    return (TreeNode)enumer.Current;
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Indent(string text, int chars) {
            return text.Replace("\n", '\n' + new string(' ', chars));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string LocalLink(TreeNode to) {
            return new StringBuilder(to.Name).Append(((ObjectInfo)to.Tag).Kind < Kinds.Functions ? "\\index." : ".")
                .Append(Design.Extension).ToString();
        }

        public static void MakeNodeName(TreeNode node, Visibility vis) {
            if (node.Name.Equals(string.Empty)) {
                int parenthesis = node.Text.IndexOf('(');
                string nameOnly = parenthesis == -1 ? node.Text : node.Text.Substring(0, parenthesis);
                int angleBracket = nameOnly.IndexOf('<');
                if (angleBracket != -1)
                    nameOnly = nameOnly.Substring(0, angleBracket);
                int tryCount = 0;
                while (true) {
                    string tryWith = nameOnly + (tryCount++ == 0 ? string.Empty : (tryCount).ToString());
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveModifier(ref string source, ref string target, string modifier) {
            if (source.Contains(modifier)) {
                source = source.Replace(modifier, string.Empty).TrimStart();
                target += modifier;
            }
        }

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

        public static string QuickSummary(string fullSummary) {
            int summaryStart = fullSummary.IndexOf("<summary>") + 9;
            return summaryStart != 8 ? Regex.Replace(fullSummary.Substring(summaryStart, fullSummary.IndexOf("</summary>") - summaryStart)
                .Replace("<see cref=\"", string.Empty).Replace("\"/>", ""), @"\r\n?|\n", " ") : string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string QuickSummary(string source, TreeNode node) {
            return RemoveTag(ref source, "summary", node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveModifier(ref string source, string modifier) {
            if (source.Contains(modifier)) {
                source = source.Replace(modifier, string.Empty).TrimStart();
                return true;
            }
            return false;
        }

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

        public static int SpacesBefore(string text, int index) {
            int count = 0;
            for (int i = index - 1; i >= 0 && text[index] == ' '; --i)
                ++count;
            return count;
        }
    }
}
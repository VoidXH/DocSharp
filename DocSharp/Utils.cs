using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DocSharp {
    enum Kinds { Namespaces, Classes, Interfaces, Enums, Structs, Functions, Properties, Variables }
    enum Visibility { Default, Private, Protected, Internal, Public }

    struct ObjectInfo {
        public bool Exportable;
        public string Attributes, DefaultValue, Extends, Modifiers, Summary, Type;
        public Visibility Vis;
        public Kinds Kind;
    }

    static class Constants {
        public static readonly string[] modifiers = {
            "abstract ", "async ", "const ", "event ", "extern ", "new ", "override ", "partial ",
            "readonly ", "sealed ", "static ", "unsafe ", "virtual ", "volatile "
        };
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

        public static void ClearFunctionBodies(TreeNode node) {
            IEnumerator enumer = node.Nodes.GetEnumerator();
            while (enumer.MoveNext()) {
                TreeNode child = (TreeNode)enumer.Current;
                if (child.Tag != null && (((ObjectInfo)child.Tag).Kind == Kinds.Functions || ((ObjectInfo)child.Tag).Kind == Kinds.Properties))
                    child.Nodes.Clear();
                else
                    ClearFunctionBodies(child);
            }
        }

        public static void FillWithPHP(DirectoryInfo target) {
            File.Create(target.FullName + "\\index.php").Close();
            DirectoryInfo[] subdirs = target.GetDirectories();
            int subdirCount = subdirs.Length;
            for (int i = 0; i < subdirCount; ++i)
                FillWithPHP(subdirs[i]);
        }

        public static TreeNode GetNodeByText(TreeNode parent, string text) {
            IEnumerator enumer = parent.Nodes.GetEnumerator();
            while (enumer.MoveNext())
                if (((TreeNode)enumer.Current).Text.Equals(text))
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

        public static void MakeNodeName(TreeNode node) {
            if (node.Name.Equals(string.Empty)) {
                int parenthesis = node.Text.IndexOf('(');
                if (parenthesis == -1)
                    node.Name = node.Text;
                else {
                    string beginning = node.Text.Substring(0, parenthesis);
                    int tryCount = 0;
                    while (true) {
                        string tryWith = beginning + (tryCount++ == 0 ? string.Empty : (tryCount).ToString());
                        bool found = false;
                        foreach (TreeNode other in node.Parent.Nodes) {
                            if (other.Name.Equals(tryWith)) {
                                found = true;
                                break;
                            }
                        }
                        if (!found) {
                            node.Name = tryWith;
                            return;
                        }
                    }
                }
            }
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
                string path = node.Text + '\\';
                while (!linked && lookingFor != null) {
                    IEnumerator children = lookingFor.Nodes.GetEnumerator();
                    while (children.MoveNext()) {
                        if (((TreeNode)children.Current).Text.Equals(reference)) {
                            reference = "<a href=\"" + path + LocalLink((TreeNode)children.Current) + "\">" + reference + "</a>";
                            linked = true;
                            break;
                        }
                    }
                    lookingFor = lookingFor.Parent;
                    if (firstRun) {
                        path = string.Empty;
                        firstRun = false;
                    } else
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
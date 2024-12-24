using System;
using System.IO;
using System.Text;

namespace DocSharp {
    partial class Design {
        /// <summary>
        /// Create the class map file with a list of all classes and their summaries.
        /// </summary>
        /// <param name="path">Root folder path of the export</param>
        /// <param name="root">Root node for the project tree</param>
        public static void GenerateClassMap(string path, MemberNode root) {
            string baseBuild = Base(root, true).Replace(cssMarker, stylesheet);
            StringBuilder page = new(classMapCSS);
            BuildClassMap(page, root, 1);
            File.WriteAllText(Path.Combine(path, "_classmap." + extension), baseBuild.Replace(contentMarker, page.ToString()));
        }

        /// <summary>
        /// Add the class graph to the class map page's content frame.
        /// </summary>
        /// <param name="page">Page under construction, where the content shouldl be appended</param>
        /// <param name="node">Node to enumerate the children of, initially the root node, but the function is recursive</param>
        /// <param name="depth">Current header to be used, initially 1, but the function is recursive</param>
        static void BuildClassMap(StringBuilder page, MemberNode node, int depth) {
            foreach (MemberNode child in node.Nodes) {
                if (child.exportable && child.Kind < Element.Functions) {
                    bool isNamespace = child.Kind == Element.Namespaces;
                    if (!isNamespace) {
                        page.Append("<a href=\"").Append(child.GetExportPath(extension)).Append("\"><p>");
                    }
                    string tag = isNamespace ? "h" + depth : "b";
                    page.Append('<').Append(tag).Append('>').Append(child.name).Append("</").Append(tag).Append('>');
                    if (isNamespace) {
                        page.Append("<div class=\"inc\">");
                        BuildClassMap(page, child, Math.Min(depth + 1, maxHeader));
                        page.Append("</div>");
                    } else {
                        page.Append("</a><br/>").Append(Utils.QuickSummary(child.summary)).Append("</p>");
                    }
                }
            }
        }

        /// <summary>
        /// Incrementally pad deeper layers more and more to achieve a tree-like appearance.
        /// </summary>
        const string classMapCSS = "<style>.inc { padding-left: 50px; }</style>";

        /// <summary>
        /// Don't allow headers (h1, h2, h3, ...) over this value. The h4 tag makes the text smaller that p in regular HTML,
        /// so 3 is the default.
        /// </summary>
        const int maxHeader = 3;
    }
}
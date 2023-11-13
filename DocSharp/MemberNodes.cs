using System;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// Post-processing functions for a set of imported nodes.
    /// </summary>
    static class MemberNodes {
        /// <summary>
        /// Split up dot-separated namespaces to child namespaces.
        /// </summary>
        public static void Diverge(this TreeNodeCollection items) {
            for (int i = 0; i < items.Count; i++) {
                MemberNode item = (MemberNode)items[i];
                if (item.Kind != Element.Namespaces) {
                    continue;
                }
                int dot = item.Name.IndexOf('.');
                if (dot >= 0) {
                    string namespaceName = item.Name[..dot];
                    MemberNode parentNamespace = MemberNode.MakeNamespace(namespaceName);
                    parentNamespace.NodeFont = item.NodeFont;
                    item.Text = item.name = item.Name = item.Name[(dot + 1)..];
                    parentNamespace.SwapWith(item);
                    item.Nodes.Add(parentNamespace);
                }
                Diverge(item.Nodes);
            }
        }

        /// <summary>
        /// Merge namespaces that span through multiple nodes.
        /// </summary>
        public static void Merge(this TreeNodeCollection items) {
            int c = items.Count;
            for (int i = 0; i < c; i++) {
                MemberNode a = (MemberNode)items[i];
                for (int j = i + 1; j < c; j++) {
                    MemberNode b = (MemberNode)items[j];
                    if (a.Kind == Element.Namespaces && b.Kind == Element.Namespaces && a.Name.Equals(b.Name)) {
                        a.MoveItemsFrom(b);
                        items.Remove(b);
                        j--;
                        c--;
                    }
                }
            }
            for (int i = 0; i < c; i++) {
                Merge(items[i].Nodes);
            }
        }

        /// <summary>
        /// Add missing values to enums.
        /// </summary>
        /// <remarks>Call this before sorting or the numbering will be in wrong order.</remarks>
        public static void NumberEnums(this TreeNodeCollection items) {
            for (int i = 0, c = items.Count; i < c; i++) {
                if (((MemberNode)items[i]).Kind == Element.Enums) {
                    TreeNodeCollection children = items[i].Nodes;
                    int next = 0;
                    for (int j = 0, len = children.Count; j < len; j++) {
                        MemberNode child = (MemberNode)children[j];
                        if (string.IsNullOrEmpty(child.defaultValue)) {
                            child.defaultValue = next++.ToString();
                        } else if (int.TryParse(child.defaultValue, out int def)) {
                            next = def + 1;
                        } else {
                            next++;
                        }
                    }
                } else {
                    NumberEnums(items[i].Nodes);
                }
            }
        }

        /// <summary>
        /// Order the nodes alphabetically with namespaces on the top.
        /// </summary>
        public static void Sort(this TreeNodeCollection items) {
            int c = items.Count, namespaceCount = 0, otherCount = 0;
            MemberNode[] namespaces = new MemberNode[c], others = new MemberNode[c];

            for (int i = 0; i < c; i++) {
                MemberNode item = (MemberNode)items[i];
                if (item.Kind == Element.Namespaces) {
                    namespaces[namespaceCount++] = item;
                } else {
                    others[otherCount++] = item;
                }
            }
            items.Clear();

            Array.Sort(namespaces, 0, namespaceCount);
            Array.Sort(others, 0, otherCount);

            for (int i = 0; i < namespaceCount; i++) {
                items.Add(namespaces[i]);
            }
            for (int i = 0; i < otherCount; i++) {
                items.Add(others[i]);
            }

            for (int i = 0; i < c; i++) {
                Sort(items[i].Nodes);
            }
        }

        /// <summary>
        /// Finds the &lt;inheritdoc/&gt; tags, and replaces them with the summary that should be inherited.
        /// </summary>
        public static void FillSummaries(this TreeNodeCollection items) {
            foreach (MemberNode item in items) {
                Process(item);
            }

            static void Process(MemberNode node) {
                foreach (MemberNode child in node.Nodes) {
                    Process(child);
                    if (child.summary.StartsWith("<inheritdoc/>")) {
                        Utils.InheritDocumentation(child);
                    }
                }
            }
        }
    }
}
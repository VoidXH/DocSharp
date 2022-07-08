using System;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// Member node list processing.
    /// </summary>
    static class MemberNodes {
        /// <summary>
        /// Split up dot-separated namespaces to child namespaces.
        /// </summary>
        public static void Diverge(this TreeNodeCollection items) {
            for (int i = 0; i < items.Count; ++i) {
                MemberNode item = (MemberNode)items[i];
                if (item.kind != Element.Namespaces)
                    continue;
                int dot = item.Name.IndexOf('.');
                if (dot >= 0) {
                    string namespaceName = item.Name.Substring(0, dot);
                    MemberNode parentNamespace = MemberNode.MakeNamespace(namespaceName);
                    parentNamespace.NodeFont = item.NodeFont;
                    item.Text = item.name = item.Name = item.Name.Substring(dot + 1);
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
            for (int i = 0; i < c; ++i) {
                MemberNode a = (MemberNode)items[i];
                for (int j = i + 1; j < c; ++j) {
                    MemberNode b = (MemberNode)items[j];
                    if (a.kind == Element.Namespaces && b.kind == Element.Namespaces && a.Name.Equals(b.Name)) {
                        a.MoveItemsFrom(b);
                        items.Remove(b);
                        --j;
                        --c;
                    }
                }
            }
            for (int i = 0; i < c; ++i)
                Merge(items[i].Nodes);
        }

        /// <summary>
        /// Order the nodes alphabetically with namespaces on the top.
        /// </summary>
        public static void Sort(this TreeNodeCollection items) {
            int c = items.Count, namespaceCount = 0, otherCount = 0;
            MemberNode[] namespaces = new MemberNode[c], others = new MemberNode[c];

            for (int i = 0; i < c; ++i) {
                MemberNode item = (MemberNode)items[i];
                if (item.kind == Element.Namespaces)
                    namespaces[namespaceCount++] = item;
                else
                    others[otherCount++] = item;
            }
            items.Clear();

            Array.Sort(namespaces, 0, namespaceCount);
            Array.Sort(others, 0, otherCount);

            for (int i = 0; i < namespaceCount; ++i)
                items.Add(namespaces[i]);
            for (int i = 0; i < otherCount; ++i)
                items.Add(others[i]);

            for (int i = 0; i < c; ++i)
                Sort(items[i].Nodes);
        }
    }
}
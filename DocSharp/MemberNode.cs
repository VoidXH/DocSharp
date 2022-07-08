using System;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// A <see cref="TreeNode"/> with program structure member data.
    /// </summary>
    class MemberNode : TreeNode, IComparable<MemberNode> {
        public bool exportable;
        public string name;
        public string attributes;
        public string defaultValue;
        public string extends;
        public string modifiers;
        public string summary;
        public string type;
        public Visibility vis;
        public Visibility getter;
        public Visibility setter;
        public Element kind;
        public ExportInfo export;

        /// <summary>
        /// Create a member with a set header.
        /// </summary>
        public MemberNode(string text) : base(text) { }

        public static MemberNode MakeNamespace(string text) {
            MemberNode node = new MemberNode(text) {
                kind = Element.Namespaces,
                name = text,
                Name = text,
                Text = text,
                attributes = string.Empty,
                defaultValue = string.Empty,
                extends = string.Empty,
                modifiers = string.Empty,
                summary = string.Empty,
                type = string.Empty
            };
            return node;
        }

        /// <summary>
        /// Copy the values of another node.
        /// </summary>
        public void CopyFrom(MemberNode other, bool copyName = true) {
            exportable = other.exportable;
            attributes = other.attributes;
            defaultValue = other.defaultValue;
            extends = other.extends;
            modifiers = other.modifiers;
            summary = other.summary;
            type = other.type;
            vis = other.vis;
            getter = other.getter;
            setter = other.setter;
            kind = other.kind;
            export = other.export;
            if (copyName) {
                name = other.name;
                Name = other.Name;
                Text = other.Text;
            }
        }

        /// <summary>
        /// Steals the children of another node.
        /// </summary>
        public void MoveItemsFrom(MemberNode other) {
            int count = other.Nodes.Count;
            while (count-- != 0) {
                MemberNode moved = (MemberNode)other.Nodes[count];
                other.Nodes.RemoveAt(count);
                Nodes.Add(moved);
            }
        }

        /// <summary>
        /// Swaps the values of two code tree items.
        /// </summary>
        public void SwapWith(MemberNode other) {
            MemberNode temp = new MemberNode(other.Text);
            temp.CopyFrom(other);
            other.CopyFrom(this);
            CopyFrom(temp);

            temp.MoveItemsFrom(other);
            other.MoveItemsFrom(this);
            MoveItemsFrom(temp);
        }

        /// <summary>
        /// Compare members by name.
        /// </summary>
        public int CompareTo(MemberNode other) => name.CompareTo(other.name);
    }
}
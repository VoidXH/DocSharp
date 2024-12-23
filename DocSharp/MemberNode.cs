﻿using System;
using System.IO;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// A <see cref="TreeNode"/> with program structure member data.
    /// </summary>
    class MemberNode(string text) : TreeNode(text), IComparable<MemberNode> {
        /// <summary>
        /// Filled by the <see cref="Exporter"/>, marks a node to be present in the generated web documentation.
        /// </summary>
        public bool exportable;

        /// <summary>
        /// Name of the element, without UML marking. The UML-compatible name is under <see cref="TreeNode.Name"/>.
        /// </summary>
        public string name;
        public string attributes;
        public string defaultValue;
        public string extends;
        public string modifiers;
        public string summary;
        public string type;

        public Visibility Vis { get; set; }
        public Visibility Getter { get; set; }
        public Visibility Setter { get; set; }
        public Element Kind { get; set; }

        public ExportInfo export;

        public static MemberNode MakeNamespace(string text) {
            MemberNode node = new(text) {
                Kind = Element.Namespaces,
                name = text,
                Name = text,
                Text = text,
                attributes = string.Empty,
                defaultValue = string.Empty,
                extends = string.Empty,
                modifiers = string.Empty,
                summary = string.Empty,
                type = Parser._namespace,
                export = new ExportInfo()
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
            Vis = other.Vis;
            Getter = other.Getter;
            Setter = other.Setter;
            Kind = other.Kind;
            export = other.export;
            if (copyName) {
                name = other.name;
                Name = other.Name;
                Text = other.Text;
            }
        }

        /// <summary>
        /// Get the path where this node will be exported from the root folder.
        /// </summary>
        public string GetExportPath(string extension) {
            string result = "index." + extension;
            TreeNode node = this;
            do {
                result = Path.Combine(((MemberNode)node).name, result);
                node = node.Parent;
            } while (node != null && node.Parent != null);
            return result;
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
            MemberNode temp = new(other.Text);
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
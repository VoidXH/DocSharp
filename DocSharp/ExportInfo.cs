using System.Collections.Generic;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// Contains preprocessed data for node export.
    /// </summary>
    public class ExportInfo {
        /// <summary>
        /// Nodes that link this node in their summaries.
        /// </summary>
        public HashSet<TreeNode> referencedBy = new HashSet<TreeNode>();

        /// <summary>
        /// Description of the element.
        /// </summary>
        public string summary;

        /// <summary>
        /// Description of the returned value of the element.
        /// </summary>
        public string returns;
    }
}
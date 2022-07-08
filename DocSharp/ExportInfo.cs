using System.Collections.Generic;

namespace DocSharp {
    /// <summary>
    /// Contains preprocessed data for node export.
    /// </summary>
    class ExportInfo {
        /// <summary>
        /// Nodes that link this node in their summaries.
        /// </summary>
        public HashSet<MemberNode> referencedBy = new HashSet<MemberNode>();

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
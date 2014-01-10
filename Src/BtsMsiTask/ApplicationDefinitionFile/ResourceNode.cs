using System.Collections.Generic;

namespace BtsMsiTask.ApplicationDefinitionFile
{
    internal class ResourceNode
    {
        internal string Type { get; set; }
        internal string Luid { get; set; }

        internal List<PropertyNode> Properties;
        internal List<FileNode> Files;

        public ResourceNode()
        {
            Properties = new List<PropertyNode>();
            Files = new List<FileNode>();
        }
    }
}

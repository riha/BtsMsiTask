using System.Xml.Serialization;

namespace BtsMsiTask.ApplicationDefinitionFile
{
    [XmlRoot(ElementName = "Reference")]
    internal class ReferenceNode
    {
        internal string Name { get; set; }
    }
}

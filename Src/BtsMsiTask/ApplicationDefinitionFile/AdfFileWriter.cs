using BtsMsiTask.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BtsMsiTask.ApplicationDefinitionFile
{
    public class AdfFileWriter
    {
        private static readonly XNamespace AdfNs = "http://Microsoft.BizTalk.ApplicationDeployment/ApplicationDefinition.xsd";
        private static readonly XNamespace XsiNs = "http://www.w3.org/2001/XMLSchema-instance";
        private static readonly XNamespace XsdNs = "http://www.w3.org/2001/XMLSchema";

        private XElement CreateReferenceXElement(string reference)
        {
            return new XElement(AdfNs + "Reference", new XAttribute("Name", reference));
        }

        private XElement CreatePropertyXElement(PropertyNode propertyNode)
        {
            if (propertyNode.Value == null)
                propertyNode.Value = string.Empty;

            return new XElement(AdfNs + "Property", new XAttribute("Name", propertyNode.Name), new XAttribute("Value", propertyNode.Value));
        }

        private XElement CreateFileXElement(FileNode fileNode)
        {
            return new XElement(AdfNs + "File", new XAttribute("RelativePath", fileNode.RelativePath), new XAttribute("Key", fileNode.Key));
        }

        public string Write(IEnumerable<BizTalkAssemblyResource> resources, string applicationName, string description, IEnumerable<string> references, string version)
        {
            var propertyNodes = GetPropertyNodes(applicationName, description, version);
            var resourceNodes = GetResourceNodes(resources);
            var resourceXElements = new List<XElement>();

            foreach (var resourceNode in resourceNodes)
            {
                var resourceXElement = new XElement(AdfNs + "Resource", new XAttribute("Type", resourceNode.Type),
                    new XAttribute("Luid", resourceNode.Luid));

                var resourcePropertyXElements = resourceNode.Properties.Select(CreatePropertyXElement);
                var resourceFileXElements = resourceNode.Files.Select(CreateFileXElement);

                resourceXElement.Add(new XElement(AdfNs + "Properties", resourcePropertyXElements),
                    new XElement(AdfNs + "Files", resourceFileXElements));

                resourceXElements.Add(resourceXElement);
            }

            var content = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(AdfNs + "ApplicationDefinition",
                    new XAttribute(XNamespace.Xmlns + "xsd", XsdNs),
                    new XAttribute(XNamespace.Xmlns + "xsi", XsiNs),
                    new XElement(AdfNs + "Properties", propertyNodes.Select(CreatePropertyXElement)),
                    new XElement(AdfNs + "Resources", resourceXElements),
                    new XElement(AdfNs + "References", references.Select(CreateReferenceXElement))));

            var folderPath = string.Concat(Path.GetTempPath(), Guid.NewGuid());

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, "adf.xml");

            content.Save(filePath);

            return filePath;
        }

        private IEnumerable<ResourceNode> GetResourceNodes(IEnumerable<BizTalkAssemblyResource> resources)
        {
            var resourceNodes = new List<ResourceNode>();
            foreach (var resource in resources)
            {
                var propertyNodes = new List<PropertyNode>();
                var fileNodes = new List<FileNode>();
                var dateText = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ssZ");

                propertyNodes.Add(new PropertyNode() { Name = "UpdateGac", Value = "True" });
                propertyNodes.Add(new PropertyNode() { Name = "Gacutil", Value = "True" });
                propertyNodes.Add(new PropertyNode() { Name = "Attributes", Value = "Archive" });
                propertyNodes.Add(new PropertyNode() { Name = "UpdateGacOnImport", Value = "True" });
                propertyNodes.Add(new PropertyNode() { Name = "Fullname", Value = resource.FullName });
                propertyNodes.Add(new PropertyNode() { Name = "UpdateOrchestrationStatus", Value = "True" });
                propertyNodes.Add(new PropertyNode() { Name = "RestartHostInstances", Value = "False" });
                propertyNodes.Add(new PropertyNode() { Name = "SourceLocation", Value = resource.AssemblyFilePath });
                propertyNodes.Add(new PropertyNode() { Name = "DestinationLocation", Value = string.Concat(@"%BTAD_InstallDir%\", Path.GetFileName(resource.AssemblyFilePath)) });
                propertyNodes.Add(new PropertyNode() { Name = "CreationTime", Value = dateText });
                propertyNodes.Add(new PropertyNode() { Name = "LastAccessTime", Value = dateText });
                propertyNodes.Add(new PropertyNode() { Name = "LastWriteTime", Value = dateText });
                propertyNodes.Add(new PropertyNode() { Name = "ShortCabinetName", Value = resource.ShortCabinetName });

                fileNodes.Add(new FileNode() { Key = "Assembly", RelativePath = Path.GetFileName(resource.AssemblyFilePath) });

                resourceNodes.Add(new ResourceNode()
                {
                    Luid = resource.FullName,
                    Type = resource.Type,
                    Properties = propertyNodes,
                    Files = fileNodes
                });
            }

            return resourceNodes;
        }

        private IEnumerable<PropertyNode> GetPropertyNodes(string displayName, string description, string version)
        {
            var propertyElements = new List<PropertyNode>
            {
                new PropertyNode() {Name = "DisplayName", Value = displayName},
                new PropertyNode() {Name = "Guid", Value = Guid.NewGuid().ToString("B").ToUpperInvariant()},
                new PropertyNode() {Name = "Manufacturer", Value = "Generated by BizTalk"},
                new PropertyNode() {Name = "Version", Value = version}, 
                new PropertyNode() {Name = "ApplicationDescription", Value = description}
            };

            return propertyElements;
        }
    }
}

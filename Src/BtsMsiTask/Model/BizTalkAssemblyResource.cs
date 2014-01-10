using BtsMsiTask.Utilities;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace BtsMsiTask.Model
{
    public class BizTalkAssemblyResource
    {
        public string FullName { get; private set; }
        public string AssemblyFilePath { get; set; }
        public string Type = "System.BizTalk:BizTalkAssembly";
        public string ShortCabinetName { get; set; }

        public BizTalkAssemblyResource(string assemblyFilePath)
        {
            AssemblyFilePath = assemblyFilePath;
            var assembly = Assembly.LoadFrom(assemblyFilePath);
            
            FullName = assembly.GetName().FullName;
        }

        internal static string GetLuidHash(string luid)
        {
            var assemblyName = new AssemblyName(luid);
            byte[] publicKeyToken = assemblyName.GetPublicKeyToken();
            var stringBuilder = new StringBuilder();

            foreach (byte num in publicKeyToken)
                stringBuilder.Append(num.ToString("x2", CultureInfo.InvariantCulture));
            
            string hash = Hasher.HashAssemblyFullyQualifiedName(assemblyName.Name, assemblyName.Version + "-" + stringBuilder).ToString("N");

            return hash;
        }

        internal string GetResourceFolder(string applicationFolder)
        {
            string resourceType = Type.Substring(Type.IndexOf(':') + 1);
            string resourceTypeFolder = GetResourceTypeFolder(applicationFolder, resourceType);
            string filename = GetLuidHash(FullName);
            string luidFilename = FileHelper.GetLuidFilename(filename);

            return Path.Combine(resourceTypeFolder, luidFilename);
        }

        internal string GetResourceTypeFolder(string applicationFolder, string resourceType)
        {
            string validFilename = FileHelper.GetValidFilename(resourceType.Substring(resourceType.IndexOf(':') + 1));

            return Path.Combine(applicationFolder, validFilename);
        }
    }
}

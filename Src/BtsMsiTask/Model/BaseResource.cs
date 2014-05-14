using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BtsMsiTask.Utilities;

namespace BtsMsiTask.Model
{
    public abstract class BaseResource
    {
        public string FullName { get; private set; }
        public string AssemblyFilePath { get; set; }
        public string ShortCabinetName { get; set; }
        public string Type { get; private set; }

        protected BaseResource(string assemblyFilePath, string type)
        {
            Type = type;

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

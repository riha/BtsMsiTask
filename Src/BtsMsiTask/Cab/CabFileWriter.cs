using BtsMsiTask.Model;
using Microsoft.Deployment.Compression.Cab;
using System;
using System.Collections.Generic;
using System.IO;

namespace BtsMsiTask.Cab
{
    internal class CabFileWriter
    {
        internal string Write(IEnumerable<BaseResource> resources)
        {
            var index = 0;

            var tempCabFolderPath = string.Concat(Path.GetTempPath(), Guid.NewGuid());

            if (!Directory.Exists(tempCabFolderPath))
                Directory.CreateDirectory(tempCabFolderPath);

            foreach (var resource in resources)
            {
                var resourceTempFolderPath = Path.GetTempPath() + Guid.NewGuid();
                var resourceFolder = resource.GetResourceFolder(resourceTempFolderPath);

                if (!Directory.Exists(resourceFolder))
                    Directory.CreateDirectory(resourceFolder);

                if (string.IsNullOrEmpty(resource.AssemblyFilePath))
                    throw new ArgumentException("AssemblyFilePath can't be empty");

                File.Copy(resource.AssemblyFilePath, Path.Combine(resourceFolder, Path.GetFileName(resource.AssemblyFilePath)));

                var cabFileName = string.Format("ITEM~{0}.cab", index);

                resource.ShortCabinetName = cabFileName;

                index = index + 1;

                var cabInfo = new CabInfo(Path.Combine(tempCabFolderPath, cabFileName));
                cabInfo.Pack(resourceTempFolderPath, true, Microsoft.Deployment.Compression.CompressionLevel.Normal, null);
            }

            return tempCabFolderPath;
        }
    }
}

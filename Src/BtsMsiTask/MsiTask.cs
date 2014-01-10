using BtsMsiTask.ApplicationDefinitionFile;
using BtsMsiTask.Cab;
using BtsMsiTask.Model;
using BtsMsiTask.Msi;
using BtsMsiTask.Utilities;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BtsMsiTask
{
    /// <summary>
    /// Exposes a MSBuild task for generating a MSI package for a BziTalk solution
    /// </summary>
    public class MsiTask : Task
    {
        /// <summary>
        /// A full path to where the MSI final should be written.
        /// </summary>
        [Required]
        public string DestinationPath { get; set; }

        /// <summary>
        /// The name of the BizTalk application, used when importing the MSI in BizTalk.
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Optional BizTalk Application description
        /// </summary>
        public string ApplicationDescription { get; set; }

        /// <summary>
        /// Optional version, will be added to the MSI properties
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// All the dll an other resources that should be packed as part of the MSI.
        /// </summary>
        [Required]
        public ITaskItem[] Resources { get; set; }

        /// <summary>
        /// Possible references to other BizTalk Application
        /// </summary>
        public ITaskItem[] References { get; set; }

        /// <summary>
        /// MSBuild entry point for generating a MSI file based on a BizTalk Server project.
        /// </summary>
        public override bool Execute()
        {
            Version version;
            try
            {
                version = !string.IsNullOrEmpty(Version) ? new Version(Version) : new Version("1.0.0.0");
            }
            catch (FormatException)
            {
                Log.LogError("Version format {0} is invalid, needs to be in format of 1.0.0.0", Version);
                throw;
            }

            if (!Directory.Exists(DestinationPath))
                Directory.CreateDirectory((DestinationPath));
            
            // TODO: Add all checking of parameters to separate classes
            // TODO: Check all required in parameters and set possible defaults
            // TODO: Check that it's BT 2013 server from registry
            // TODO: Is it possible to check if assembly is signed or not?
            // TODO: Check that these are only BizTalk resources for now

            var resources = Resources.Select(r => new BizTalkAssemblyResource(r.GetMetadata("Fullpath"))).ToList();

            var references = new List<string> { "BizTalk.System" };
            if (References != null)
                references.AddRange(References.Select(reference => reference.ItemSpec));

            var cabFileWriter = new CabFileWriter();
            var cabFolderPath = cabFileWriter.Write(resources);

            var adfFileWriter = new AdfFileWriter();
            var adfFilePath = adfFileWriter.Write(resources, ApplicationName, ApplicationDescription, references, version.ToString());

            var destinationFilePath = Path.Combine(DestinationPath, FileHelper.GetMsiFileName(ApplicationName));
            MsiFileWriter.Write(destinationFilePath);

            var productCode = Guid.NewGuid();
            var upgradeCode = Guid.NewGuid();
            var properties = MsiFileWriter.GetProperties(ApplicationName, version, productCode, upgradeCode);
            using (var db = new Database(destinationFilePath, DatabaseOpenMode.Direct))
            {
                db.UpdateSummaryInfo();
                db.UpdateUpgradeTable(upgradeCode);
                db.UpdateProperties(properties);
                db.UpdateFileContent(cabFolderPath, adfFilePath, resources.Count());
                db.MakeCustomModifications(productCode, ApplicationName);
                db.Commit();
            }

            return true;
        }
    }
}

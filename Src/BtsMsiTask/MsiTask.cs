using System.Reflection;
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
        /// Full path to the final destination of the MSI. In a build server scenario this would the folder that's being compiled to.
        /// </summary>
        [Required]
        public string DestinationPath { get; set; }

        /// <summary>
        /// The name of the BizTalk application. Used when importing the MSI in BizTalk to either create a new application or update an existing.
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Optional BizTalk Application description. Updated or created when importing MSI.
        /// </summary>
        public string ApplicationDescription { get; set; }

        /// <summary>
        /// Optional version, will be added to the MSI properties.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Optional file name. If set if will dictate the MSI file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Optional source location path. If set if will be part of the MSI property for source location and visible in the BizTalk Administration console.
        /// </summary>
        public string SourceLocation { get; set; }

        /// <summary>
        /// All the BizTalk dlls that should be packed as part of the MSI.
        /// </summary>
        public ITaskItem[] BtsAssemblies { get; set; }

        /// <summary>
        /// All the non BizTalk dlls that should be packed as part of the MSI.
        /// </summary>
        public ITaskItem[] Resources { get; set; }

        /// <summary>
        /// Possible references to other BizTalk Application
        /// </summary>
        public ITaskItem[] ReferenceApplications { get; set; }

        /// <summary>
        /// MSBuild entry point for generating a MSI file based on a BizTalk Server project.
        /// </summary>
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Executing MsiTask version {0}", Assembly.GetExecutingAssembly().GetName().Version);

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

            if (!string.IsNullOrEmpty(FileName) && Path.GetExtension(FileName) != ".msi")
            {
                Log.LogError("MSI file name has to end with file suffix '.MSI'", Version);
                return false;
            }

            if ((BtsAssemblies == null || !BtsAssemblies.Any()) && (Resources == null || !Resources.Any()))
            {
                throw new ArgumentException("No BizTalk Assemblies and no assembly resources found in build project input");
            }

            // TODO: Add all checking of parameters to separate classes
            // TODO: Check all required in parameters and set possible defaults
            // TODO: Check that it's BT 2013 server from registry
            // TODO: Is it possible to check if assembly is signed or not?

            var resources = new List<BaseResource>();

            if (BtsAssemblies != null && BtsAssemblies.Any())
                resources.AddRange(BtsAssemblies.Select(r => new BizTalkAssemblyResource(r.GetMetadata("Fullpath"))));

            if (Resources != null && Resources.Any())
                resources.AddRange(Resources.Select(r => new AssemblyResource(r.GetMetadata("Fullpath"))));

            var references = new List<string> { "BizTalk.System" };
            if (ReferenceApplications != null)
                references.AddRange(ReferenceApplications.Select(reference => reference.ItemSpec));

            var cabFileWriter = new CabFileWriter();
            var cabFolderPath = cabFileWriter.Write(resources);

            var adfFileWriter = new AdfFileWriter();
            var adfFilePath = adfFileWriter.Write(resources, ApplicationName, ApplicationDescription, references, version.ToString(), SourceLocation);

            var destinationFilePath = Path.Combine(DestinationPath, FileHelper.GetMsiFileName(ApplicationName, FileName));
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

            Log.LogMessage(MessageImportance.Normal, "MSI was successfully generated at {0}", destinationFilePath);

            return true;
        }
    }
}

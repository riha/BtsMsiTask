﻿using BtsMsiTask.ApplicationDefinitionFile;
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
        /// Optional build number. If set if will be part of the MSI file name.
        /// </summary>
        public string BuildNumber { get; set; }

        /// <summary>
        /// Optional source location path. If set if will be part of the MSI property for source location and visible in the BizTalk Administration console.
        /// </summary>
        public string SourceLocation { get; set; }

        /// <summary>
        /// All the dll an other resources that should be packed as part of the MSI.
        /// </summary>
        public ITaskItem[] BtsAssemblies { get; set; }

        /// <summary>
        /// 
        /// </summary>
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

            if (!BtsAssemblies.Any() && !Resources.Any())
            {
                throw new ArgumentException("No BizTalk Assemblies and no assembly resources found in build project input");
            }

            // TODO: Add all checking of parameters to separate classes
            // TODO: Check all required in parameters and set possible defaults
            // TODO: Check that it's BT 2013 server from registry
            // TODO: Is it possible to check if assembly is signed or not?
            // TODO: Check that these are only BizTalk resources for now

            var resources = BtsAssemblies.Select(r => new BizTalkAssemblyResource(r.GetMetadata("Fullpath"))).ToList<BaseResource>();
            resources.AddRange(Resources.Select(r => new AssemblyResource(r.GetMetadata("Fullpath"))).ToList());

            var references = new List<string> { "BizTalk.System" };
            if (References != null)
                references.AddRange(References.Select(reference => reference.ItemSpec));

            var cabFileWriter = new CabFileWriter();
            var cabFolderPath = cabFileWriter.Write(resources);

            var adfFileWriter = new AdfFileWriter();
            var adfFilePath = adfFileWriter.Write(resources, ApplicationName, ApplicationDescription, references, version.ToString(), SourceLocation);

            var destinationFilePath = Path.Combine(DestinationPath, FileHelper.GetMsiFileName(ApplicationName, BuildNumber));
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

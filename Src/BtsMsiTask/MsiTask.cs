using System.Reflection;
using BtsMsiLib.Model;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
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
        /// Path to the final destination of the MSI. 
        /// In a build server scenario this would the folder that's being compiled to.
        /// Example: 'C:\Temp\BtsSample'
        /// </summary>
        [Required]
        public string DestinationPath { get; set; }

        /// <summary>
        /// The name of the BizTalk application. 
        /// Used when importing the MSI in BizTalk to either create a new application or to match and update an existing.
        /// Example: 'BtsSampleApp'
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Optional BizTalk Application description. 
        /// Updated or created when importing MSI.
        /// </summary>
        public string ApplicationDescription { get; set; }

        /// <summary>
        /// Optional version, will be added to the MSI properties.
        /// Deprecated. Will be ignored. 
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Optional file name. If set if will dictate the MSI file name.
        /// Example: 'Build 23456_2.msi'
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Optional source location path. 
        /// If set if will be part of the MSI property for source location and visible in the BizTalk Administration console.
        /// Example: '\\acme.com\drops$\Build 23456_2'
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

            if (!Directory.Exists(DestinationPath))
                Directory.CreateDirectory((DestinationPath));

            if (!string.IsNullOrEmpty(FileName) && Path.GetExtension(FileName) != ".msi")
            {
                Log.LogError("MSI file name has to end with file suffix '.MSI'");
                return false;
            }

            if ((BtsAssemblies == null || !BtsAssemblies.Any()) && (Resources == null || !Resources.Any()))
            {
                throw new ArgumentException("No BizTalk Assemblies and no assembly resources found in build project input");
            }

            // TODO: Add all checking of parameters to separate classes
            // TODO: Check all required in parameters and set possible defaults
            // TODO: Check that it's BT 2013 server from registry
            // TODO: Add better and cleaner error messages to msbuild output
            // TODO: Is it possible to check if assembly is signed or not?

            var resources = new List<Resource>();

            if (BtsAssemblies != null && BtsAssemblies.Any())
                resources.AddRange(BtsAssemblies.Select(r => new Resource(r.GetMetadata("Fullpath"), ResourceType.BtsResource)));

            if (Resources != null && Resources.Any())
                resources.AddRange(Resources.Select(r => new Resource(r.GetMetadata("Fullpath"), ResourceType.Resource)));

            var references = new List<string>();
            if (ReferenceApplications != null)
                references.AddRange(ReferenceApplications.Select(reference => reference.ItemSpec));

            var btsApplication = new BtsApplication(ApplicationName)
            {
                Description = ApplicationDescription,
                ReferencedApplications = references.ToArray()
            };
            
            var msiWriter = new BtsMsiLib.MsiWriter();
            var msiFile = msiWriter.Write(btsApplication, resources.ToArray());

            var destinationFilePath = Path.Combine(DestinationPath, FileHelper.GetMsiFileName(ApplicationName, FileName));

            using (var destinationFile = File.Create(destinationFilePath)) { msiFile.CopyTo(destinationFile); }

            Log.LogMessage(MessageImportance.Normal, "MSI was successfully generated at {0}", destinationFilePath);

            return true;
        }
    }
}

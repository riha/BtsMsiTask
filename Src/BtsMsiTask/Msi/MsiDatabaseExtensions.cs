using BtsMsiTask.Utilities;
using Microsoft.Deployment.Compression.Cab;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BtsMsiTask.Msi
{
    //TODO: User better variable names in general
    public static class MsiDatabaseExtensions
    {
        public static void UpdateSummaryInfo(this Database db)
        {
            // TODO: Correct values needs to be set
            using (SummaryInfo summaryInfo = db.SummaryInfo)
            {
                summaryInfo.Title = "A";
                summaryInfo.Author = "B";
                summaryInfo.Subject = "C";
                summaryInfo.Comments = "D";
                summaryInfo.Keywords = "BizTalk, deployment, application, " + "sdfsdWfsdf";
                summaryInfo.RevisionNumber = Guid.NewGuid().ToString("B").ToUpperInvariant();
                summaryInfo.CreatingApp = typeof(MsiDatabaseExtensions).Assembly.FullName;
                summaryInfo.CreateTime = DateTime.Now;
                summaryInfo.Persist();
            }
        }

        public static void UpdateUpgradeTable(this Database db, Guid upgradeCode)
        {
            using (View view = db.OpenView("SELECT * FROM `Upgrade`", new object[0]))
            {
                view.Execute();
                using (Record record = view.Fetch())
                {
                    record[1] = upgradeCode.ToString("B").ToUpperInvariant();
                    view.Replace(record);
                }

                db.Commit();
            }
        }

        public static void UpdateProperties(this Database db, IDictionary<string, object> properties)
        {
            using (View view = db.OpenView("SELECT * FROM `Property`", new object[0]))
            {
                view.Execute();
                foreach (string index in properties.Keys)
                {
                    using (var record = new Record(2))
                    {
                        record[1] = index;
                        record[2] = properties[index];
                        view.Assign(record);
                    }
                }

                db.Commit();
            }
        }

        public static void UpdateFileContent(this Database db, string folder, string adfFile, int resourceCount)
        {
            using (View view1 = db.OpenView("SELECT * FROM `File` WHERE `FileName` = 'APPLIC~1.ADF|ApplicationDefinition.adf'", new object[0]))
            {
                view1.Execute();
                using (Record record1 = view1.Fetch())
                {
                    string path2_1 = (string)record1[1];
                    Guid guid = Guid.NewGuid();
                    string path2_2 = "_" + guid.ToString("N").ToUpperInvariant();
                    string str1 = "C_" + path2_2;
                    string mediaStream = ExtractMediaStream(db);

                    File.Delete(Path.Combine(mediaStream, path2_1));

                    File.Copy(adfFile, Path.Combine(mediaStream, path2_2));
                    using (View view2 = db.OpenView("SELECT * FROM `Component` WHERE `KeyPath` = '{0}'", new object[]
                      {
                        path2_1
                      }))
                    {
                        view2.Execute();
                        using (Record record2 = view2.Fetch())
                        {
                            record2[1] = str1;
                            record2[2] = guid.ToString("B").ToUpperInvariant();
                            record2[6] = path2_2;
                            record2[3] = "ADFDIR";
                            view2.Replace(record2);
                        }
                    }
                    using (View view2 = db.OpenView("SELECT * FROM `FeatureComponents` WHERE `Component_` = '{0}'", new object[] { "C_" + path2_1 }))
                    {
                        view2.Execute();
                        using (Record record2 = view2.Fetch())
                        {
                            record2[2] = str1;
                            view2.Assign(record2);
                        }
                    }
                    record1[1] = path2_2;
                    record1[2] = str1;
                    record1[4] = FileHelper.FileSize(adfFile);
                    view1.Replace(record1);
                    var num = (int)db.ExecuteScalar("SELECT `LastSequence` FROM `Media` WHERE `DiskId` = {0}", new object[] { 1 });

                    for (int index = 0; index < resourceCount; ++index)
                    {
                        string cabFileName = string.Format(CultureInfo.InvariantCulture, "ITEM~{0}.CAB", new object[]{index});
                        string cabFilePath = Path.Combine(folder, cabFileName);
                        record1[1] = "_" + Guid.NewGuid().ToString("N").ToUpperInvariant();
                        record1[3] = string.Format(CultureInfo.InvariantCulture, "{0}|{1}", new object[]
                        {
                            cabFileName,
                            cabFileName
                        });
                        record1[4] = FileHelper.FileSize(cabFilePath);
                        record1[8] = ++num;
                        view1.Assign(record1);
                        File.Copy(cabFilePath, Path.Combine(mediaStream, (string)record1[1]));
                    }

                    UpdateMediaCab(db, mediaStream, num);
                }
            }
        }

        private static string ExtractMediaStream(Database db)
        {
            string tempFileName = Path.GetTempFileName();
            string tempFolder = FileHelper.GetTempFolder(tempFileName);
            string cabTempFileName = Path.ChangeExtension(tempFileName, ".cab");
            var cabFileName = (string)db.ExecuteScalar("SELECT `Cabinet` FROM `Media` WHERE `DiskId` = {0}", new object[] { 1 });

            using (View view = db.OpenView("SELECT `Name`, `Data` FROM `_Streams` WHERE `Name` = '{0}'", new object[] { cabFileName.Substring(1) }))
            {
                view.Execute();
                
                Record record = view.Fetch();
                
                if (record == null)
                    throw new InstallerException("Stream not found: " + cabFileName);

                using (record)
                    record.GetStream("Data", cabTempFileName);
            }
            var cabinetInfo = new CabInfo(cabTempFileName);
            cabinetInfo.Unpack(tempFolder);
            cabinetInfo.Delete();
            return tempFolder;
        }

        private static void UpdateMediaCab(Database msiDb, string folderToPack, int lastSequence)
        {
            string tempFileName = Path.GetTempFileName();

            if (File.Exists(tempFileName))
                File.Delete(tempFileName);

            string cabFileName = Path.ChangeExtension(tempFileName, ".cab");

            new CabInfo(cabFileName).Pack(folderToPack);

            Directory.Delete(folderToPack, true);

            var list = new List<string>();
            using (View view = msiDb.OpenView("SELECT `File` FROM `File`", new object[0]))
            {
                view.Execute();
                Record record;
                while ((record = view.Fetch()) != null)
                {
                    using (record)
                        list.Add((string)record[1]);
                }
                list.Sort();
            }

            using (View view = msiDb.OpenView("SELECT `File`, `Sequence` FROM `File`", new object[0]))
            {
                view.Execute();
                for (Record record = view.Fetch(); record != null; record = view.Fetch())
                {
                    using (record)
                    {
                        record[2] = list.IndexOf((string)record[1]) + 1;
                        view.Update(record);
                    }
                }
            }

            string cabinet;
            using (View view = msiDb.OpenView("SELECT `LastSequence`, `Cabinet` FROM `Media` WHERE `DiskId` = {0}", new object[] { 1 }))
            {
                view.Execute();
                Record record = view.Fetch();
                if (record == null)
                    throw new InstallerException("Media for DiskID=1 is not found: ");
                using (record)
                {
                    cabinet = (string)record[2];
                    record[1] = lastSequence;
                    view.Update(record);
                }
            }

            using (View view = msiDb.OpenView("SELECT `Name`, `Data` FROM `_Streams` WHERE `Name` = '{0}'", new object[] { cabinet.Substring(1) }))
            {
                view.Execute();
                Record record = view.Fetch();
                using (record)
                {
                    record.SetStream("Data", cabFileName);
                    view.Update(record);
                }
            }
        }

        public static void MakeCustomModifications(this Database db, Guid productCode, string applicationName)
        {
            string productCodeUpper = productCode.ToString("B").ToUpperInvariant();
            string shortProductCode = productCodeUpper.Substring(1, productCodeUpper.Length - 2);

            using (View view = db.OpenView("SELECT * FROM `Directory`", new object[0]))
            {
                view.Execute();
                using (var record = new Record(3))
                {
                    record[1] = "ADFDIR";
                    record[2] = "TARGETDIR";
                    record[3] = shortProductCode;
                    view.Assign(record);
                }
                db.Commit();
            }

            using (View view = db.OpenView("SELECT * FROM `ControlCondition`", new object[0]))
            {
                view.Execute();
                using (var record = new Record(4))
                {
                    record[1] = "FolderForm";
                    record[2] = "DiskCostButton";
                    record[3] = "Hide";
                    record[4] = "1=1";
                    view.Assign(record);
                }
                db.Commit();
            }

            Guid applicationGuid = Hasher.HashApplicationName(applicationName);
            string applicationCompId = "C__" + applicationGuid.ToString("N").ToUpperInvariant();

            string msiCompId;
            using (View view = db.OpenView("SELECT * FROM `File` WHERE `FileName` = 'APPLIC~1.ADF|ApplicationDefinition.adf'", new object[0]))
            {
                view.Execute();
                using (Record record = view.Fetch())
                    msiCompId = record[2].ToString();
                db.Commit();
            }

            CustomModifyRegistry(db, applicationCompId, msiCompId);
            CustomModifyComponent(db, applicationGuid, applicationCompId);
            CustomModifyFeatureComponents(db, applicationCompId, msiCompId);
            CustomModifyCustomAction(db);
            ModifySecureCustomProperties(db);
            AddErrorTableEntry(db);
        }

        private static void CustomModifyRegistry(Database msiDb, string applicationCompId, string msiCompId)
        {
            using (View view = msiDb.OpenView("SELECT * FROM `Registry` WHERE `Key` = 'Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\[ProductName]\\[ProductCode]'", new object[0]))
            {
                view.Execute();
                using (Record record = view.Fetch())
                {
                    record[6] = msiCompId;
                    view.Replace(record);
                }
            }

            using (View view = msiDb.OpenView("SELECT * FROM `Registry` WHERE `Key` = 'Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\[ProductName]' AND `Name` = '*'", new object[0]))
            {
                view.Execute();
                using (Record record = view.Fetch())
                {
                    record[6] = applicationCompId;
                    view.Replace(record);
                }
            }

            CustomModifyRegistryEntry(msiDb, applicationCompId, "NoModify");
            ModifyReadmeRegistryEntry(msiDb, applicationCompId);
            CustomModifyRegistryEntry(msiDb, applicationCompId, "URLInfoAbout");
            CustomModifyRegistryEntry(msiDb, applicationCompId, "Contact");
            CustomModifyRegistryEntry(msiDb, applicationCompId, "Uninstallstring");
            CustomModifyRegistryEntry(msiDb, applicationCompId, "NoRepair");
            CustomModifyRegistryEntry(msiDb, applicationCompId, "DisplayName");

            msiDb.Commit();
        }

        private static void CustomModifyComponent(Database msiDb, Guid applicationGuid, string applicationCompId)
        {
            using (View view = msiDb.OpenView("SELECT * FROM `Component` WHERE `Directory_` = 'TARGETDIR' AND `Attributes` = 4", new object[0]))
            {
                view.Execute();
                using (Record record = view.Fetch())
                {
                    record[1] = applicationCompId;
                    record[2] = applicationGuid.ToString("B").ToUpperInvariant();
                    view.Replace(record);
                }
                while (true)
                {
                    using (Record record = view.Fetch())
                    {
                        if (record != null)
                            view.Delete(record);
                        else
                            break;
                    }
                }
                msiDb.Commit();
            }
        }

        private static void CustomModifyFeatureComponents(Database msiDb, string applicationCompId, string msiCompId)
        {
            using (View view = msiDb.OpenView("SELECT * FROM `FeatureComponents`", new object[0]))
            {
                view.Execute();
                while (true)
                {
                    using (Record record = view.Fetch())
                    {
                        if (record != null)
                            view.Delete(record);
                        else
                            break;
                    }
                }

                const string featureName = "DefaultFeature";
                
                using (var record = new Record(2))
                {
                    record[1] = featureName;
                    record[2] = applicationCompId;
                    view.Assign(record);
                }

                using (var record = new Record(2))
                {
                    record[1] = featureName;
                    record[2] = msiCompId;
                    view.Assign(record);
                }
                msiDb.Commit();
            }
        }

        private static void CustomModifyCustomAction(Database msiDb)
        {
            using (View view = msiDb.OpenView("SELECT * FROM `CustomAction` WHERE `Source` = 'InstallUtil'", new object[0]))
            {
                view.Execute();
                
                while (true)
                {
                    using (Record record = view.Fetch())
                    {
                        if (record != null)
                        {
                            record.SetInteger(2, record.GetInteger(2) | 2048);
                            view.Replace(record);
                        }
                        else
                            break;
                    }
                }

                msiDb.Commit();
            }
        }

        private static void ModifySecureCustomProperties(Database msiDb)
        {
            using (View view = msiDb.OpenView("SELECT * FROM `Property` WHERE `Property` = 'SecureCustomProperties'", new object[0]))
            {
                view.Execute();

                using (Record record = view.Fetch())
                {
                    record[2] = "NEWERPRODUCTFOUND;BTSVERSION;BTSPATH;BTSPRODUCTNAME";
                    view.Replace(record);
                }

                msiDb.Commit();
            }
        }

        private static void AddErrorTableEntry(Database msiDb)
        {
            using (View view = msiDb.OpenView("SELECT * FROM `Error`", new object[0]))
            {
                view.Execute();
                using (var record = new Record(2))
                {
                    record[1] = 1001;
                    record[2] = "Error [1]: [2]";
                    view.Assign(record);
                }
                msiDb.Commit();
            }
        }

        private static void CustomModifyRegistryEntry(Database msiDb, string compId, string name)
        {
            using (View view = msiDb.OpenView("SELECT * FROM `Registry` WHERE `Name` = '{0}'", new object[] { name }))
            {
                view.Execute();

                using (Record record = view.Fetch())
                {
                    record[6] = compId;
                    view.Replace(record);
                }
            }
        }

        private static void ModifyReadmeRegistryEntry(Database msiDb, string compId)
        {
            using (View view = msiDb.OpenView("SELECT * FROM `Registry` WHERE `Name` = '{0}'", new object[] { "Readme" }))
            {
                view.Execute();

                using (Record record = view.Fetch())
                {
                    record[6] = compId;
                    record[5] = "file://[TARGETDIR]Readme.htm";
                    view.Replace(record);
                }
            }
        }
    }
}

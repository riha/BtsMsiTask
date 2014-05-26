using System;
using System.Collections.Generic;
using System.IO;

namespace BtsMsiTask.Utilities
{
    public static class FileHelper
    {
        public static string GetMsiFileName(string applicationName, string fileName)
        {
            return !string.IsNullOrEmpty(fileName) ? fileName : string.Concat(applicationName, DateTime.Now.ToString("-yyyyMMddHHmmss"), ".msi");
        }

        public static string GetValidFilename(string filename)
        {
            var list = new List<char>();
            list.AddRange(Path.GetInvalidPathChars());
            list.Add(Path.VolumeSeparatorChar);
            list.Add(Path.DirectorySeparatorChar);
            list.Add(Path.AltDirectorySeparatorChar);
            list.Add(Path.PathSeparator);

            foreach (char oldChar in list)
                filename = filename.Replace(oldChar, '_');

            if (filename.EndsWith(".", StringComparison.Ordinal))
            {
                int length1 = filename.Length;
                filename = filename.TrimEnd(new[] { '.' });
                int length2 = filename.Length;
                filename = filename + new string('_', length1 - length2);
            }

            return filename;
        }

        public static string GetLuidFilename(string filename)
        {
            var list = new List<char>();

            list.AddRange(Path.GetInvalidPathChars());
            list.Add(Path.VolumeSeparatorChar);
            list.Add(Path.DirectorySeparatorChar);
            list.Add(Path.AltDirectorySeparatorChar);
            list.Add(Path.PathSeparator);

            foreach (char oldChar in list)
                filename = filename.Replace(oldChar, '-');

            return filename;
        }

        public static string GetTempFolder(string tmpFile)
        {
            if (File.Exists(tmpFile))
                File.Delete(tmpFile);

            string path = Path.ChangeExtension(tmpFile, null);

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            Directory.CreateDirectory(path);

            return path;
        }

        public static long FileSize(string path)
        {
            return new FileInfo(path).Length;
        }
    }
}

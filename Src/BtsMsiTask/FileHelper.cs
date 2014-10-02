using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtsMsiTask
{
    internal class FileHelper
    {
        internal static string GetMsiFileName(string applicationName, string fileName)
        {
            return !string.IsNullOrEmpty(fileName) ? fileName : string.Concat(applicationName, DateTime.Now.ToString("-yyyyMMddHHmmss"), ".msi");
        }
    }
}

using System;
using System.Security.Cryptography;
using System.Text;

namespace BtsMsiTask.Utilities
{
    public static class Hasher
    {
        public static Guid HashAssemblyFullyQualifiedName(string assemblyName, string assemblyVersionAndToken)
        {
            using (var md5 = (MD5)new MD5CryptoServiceProvider())
            {
                Guid guid = HashAssemblyName(assemblyName);
                
                byte[] numArray = guid.ToByteArray();
                
                byte[] inputBuffer1 = guid.ToByteArray();
                int length1 = inputBuffer1.GetLength(0);
                
                var outputBuffer1 = new byte[length1];
                md5.TransformBlock(inputBuffer1, 0, length1, outputBuffer1, 0);
                byte[] bytes = new UnicodeEncoding().GetBytes(assemblyVersionAndToken);
                int length2 = bytes.GetLength(0);
                
                var outputBuffer2 = new byte[length2];
                md5.TransformBlock(bytes, 0, length2, outputBuffer2, 0);
                
                var inputBuffer2 = new byte[0];
                md5.TransformFinalBlock(inputBuffer2, 0, 0);
                byte[] hash = md5.Hash;
                
                for (int index = 0; index < 9; ++index)
                {
                    hash[10 + index % 6] ^= hash[index];
                    hash[index] = numArray[index];
                }

                return new Guid(hash);
            }
        }

        internal static Guid HashAssemblyName(string assemblyName)
        {
            return Hash(assemblyName);
        }

        internal static Guid HashApplicationName(string applicationName)
        {
            return Hash(applicationName);
        }

        private static Guid Hash(string name)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] bytes = new UnicodeEncoding().GetBytes(name);
                return new Guid(md5.ComputeHash(bytes));
            }
        }
    }
}

using System;
using System.IO;
using System.Linq;

namespace Gehtsoft.Barcodes.Examples.Extensions
{
    public static class FileComparer
    {
        private static readonly string etalonDir;
        private static readonly string currentDir;

        static FileComparer()
        {
            currentDir = Directory.GetCurrentDirectory();
            etalonDir = Path.Combine(currentDir, "Content", "Etalons");
        }

        public static bool AreEqual(string expected, string actual)
        {
            string expectedFile = Path.Combine(etalonDir, expected);
            string actualFile = Path.Combine(currentDir, actual);
            byte[] a = File.ReadAllBytes(expectedFile);
            byte[] b = File.ReadAllBytes(actualFile);
            bool areEqual = File.ReadAllBytes(expectedFile).SequenceEqual(File.ReadAllBytes(actualFile));
            return areEqual; 
        }
    }
}

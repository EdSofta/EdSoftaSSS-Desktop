using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    internal class FolderClearer
    {
        public System.IO.DirectoryInfo SourceFolder { get; set; }

        public void clearAllAsync()
        {
            foreach (var directory in SourceFolder.EnumerateDirectories())
            {
                Directory.Delete(directory.FullName, true);
            }

            foreach (var sourceFile in SourceFolder.EnumerateFiles())
            {
                File.Delete(sourceFile.FullName);
            }

        }
    }
}

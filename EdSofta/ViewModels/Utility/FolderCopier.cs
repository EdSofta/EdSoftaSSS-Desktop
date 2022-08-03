using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    public class FolderCopier
    {
        public System.IO.DirectoryInfo SourceFolder { get; set; }
        public System.IO.DirectoryInfo DestinationFolder { get; set; }

        public async Task CopyAsync()
        {
            foreach (FileInfo sourceFile in SourceFolder.EnumerateFiles())
            {
                var fileCopier = new FileCopier()
                {
                    SourceFile = sourceFile,
                    DestinationFolder = this.DestinationFolder,
                };
                await fileCopier.CopyAsync();
            }
        }

        public async Task CopyAllAsync()
        {
            foreach (var directory in SourceFolder.EnumerateDirectories())
            {
                var path = Path.Combine(
                    DestinationFolder.FullName, directory.Name);

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var folderCopier = new FolderCopier
                {
                    SourceFolder = directory,
                    DestinationFolder = new DirectoryInfo(path)
                };

                await folderCopier.CopyAllAsync();
            }

            //foreach (FileInfo sourceFile in SourceFolder.EnumerateFiles())
            //{
            //    var fileCopier = new FileCopier()
            //    {
            //        SourceFile = sourceFile,
            //        DestinationFolder = this.DestinationFolder,
            //    };
            //    await fileCopier.CopyAsync();
            //}

            var filesToCopy = SourceFolder.EnumerateFiles().ToList();

            for (var i = 0; i < filesToCopy.Count; ++i)
            {
                var fileCopier = new FileCopier()
                {
                    SourceFile = filesToCopy[i],
                    DestinationFolder = this.DestinationFolder,
                };
                await fileCopier.CopyAsync();

                this.OnFileCopied(i, filesToCopy.Count);
            }

        }

        public event EventHandler<FileCopiedEventArgs> EventFileCopied;
        public void OnFileCopied(int nrOfCopiedFiles, int filesToCopy)
        {
            var tmpEvent = this.EventFileCopied;
            tmpEvent?.Invoke(this, new FileCopiedEventArgs()
            {
                DestinationFolder = this.DestinationFolder.Name,
                NrOfFilesCopied = nrOfCopiedFiles,
                NrOfFilesToCopy = filesToCopy
            });

            // instead of checking for null you can use the null-coalescent operator:
            //this.EventFileCopied?.Invoke(this, new ...);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    public class FileCopier
    {
        public System.IO.FileInfo SourceFile { get; set; }
        public System.IO.DirectoryInfo DestinationFolder { get; set; }

        public async Task CopyAsync()
        {
            string destinationFileName = Path.Combine(DestinationFolder.FullName, SourceFile.Name);

            if (File.Exists(destinationFileName)) return;

            // open source file for reading
            var source = SourceFile.FullName;
            using (Stream sourceStream = File.Open(source, FileMode.Open))
            {
                // create destination file write
                using (Stream destinationStream = File.Open(destinationFileName, FileMode.CreateNew))
                {
                    await CopyAsync(sourceStream, destinationStream);
                }
            }

        }

        public async Task CopyAsync(Stream Source, Stream Destination)
            {
                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await Source.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await Destination.WriteAsync(buffer, 0, numRead);
                }
            }
    }
}

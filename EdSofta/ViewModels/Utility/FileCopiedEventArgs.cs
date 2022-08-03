using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    public class FileCopiedEventArgs
    {
        public string DestinationFolder { get; set; }
        public int NrOfFilesCopied { get; set; }
        public int NrOfFilesToCopy { get; set; }
    }
}

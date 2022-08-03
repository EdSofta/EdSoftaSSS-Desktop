using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    internal class UpdateRequestData
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string activationKey { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string version { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string productCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    internal class Product
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string productKey { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string pin { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string phoneNumber { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string productCode { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string type { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string client { get; set; }
    }
}

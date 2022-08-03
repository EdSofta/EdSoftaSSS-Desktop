using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    public class Study
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Subject { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Topic { get; set; }
    }
}

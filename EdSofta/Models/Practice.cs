using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    public class Practice
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Subject { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<string> Topics { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    internal class ErrorReport
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string year { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string subject { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string questionNumber { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string error { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string comment { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string name { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string phoneNumber { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string client { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string type { get; set; }

    }
}

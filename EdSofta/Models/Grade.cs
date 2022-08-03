using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    internal class Grade
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string name { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public double score { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public double totalScore { get; set; }
    }
}

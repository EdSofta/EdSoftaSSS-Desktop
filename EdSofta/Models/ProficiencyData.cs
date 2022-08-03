using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    class ProficiencyData
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public double? proficiency { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<double> proficiencyHistory { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string topicName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Interfaces;

namespace EdSofta.Models
{
    class QuestionData
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Year { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int Number { get; set; }
    }
}

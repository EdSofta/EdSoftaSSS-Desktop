using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;

namespace EdSofta.Models
{
    public class SavedQuestion
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int QuestionId { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Subject { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int Number { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Year { get; set; }
        //public string Type { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Topic { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Type { get; set; }


        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string DisplayName => $"{Subject} {Year} Q{Number.ToString()}";
    }
}

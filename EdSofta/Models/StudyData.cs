using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    class StudyData
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public DateTime dateLastRead { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<DateTime> readingHistory { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string topicName { get; set; }
    }
}

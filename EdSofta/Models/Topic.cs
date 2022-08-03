using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    internal class Topic
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Name { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int TotalQuestions { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<QuestionData> Questions { get; set; }
    }
}

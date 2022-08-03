using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    internal class Result
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public double timeSpent { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public double averageTime { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public double totalTime { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<Grade> result { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public Dictionary<string, List<Grade>> topicGrades { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<QuestionData> failedQuestions { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public DateTime date { get; set; }

    }

}

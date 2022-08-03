using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Repositories
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class Question
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuestionNumber { get; set; }
        public string QuestionYear { get; set; }
        public string SubjectName { get; set; }
        public string TopicName { get; set; }
        public bool Saved { get; set; }
        public bool Reported { get; set; }
        public bool Failed { get; set; }
        public string Type { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;



namespace EdSofta.Models
{
    internal class GameQuestion
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Question { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Topic { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Answer { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Explanation { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Difficulty { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<GameOption> Options { get; set; }
    }

    internal class GameOption
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Key { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Value { get; set; }
    }
}

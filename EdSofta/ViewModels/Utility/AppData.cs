using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    internal class AppData
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string ProductKey { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string ActivationKey { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool Notifications { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Theme { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int StudyCount { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int TestCount { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool UserRegistered { get; set; }
    }
}

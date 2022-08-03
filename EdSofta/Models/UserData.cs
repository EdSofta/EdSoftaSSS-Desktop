using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    internal class UserData
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string firstName { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string surname { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string username { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string phoneNumber { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string email { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string accountType { get; set; }
    }
}

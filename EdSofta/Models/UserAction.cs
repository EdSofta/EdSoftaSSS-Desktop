using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;

namespace EdSofta.Models
{
    class UserAction
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public Guid id { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public UserActionType type { get; set; }
    }
}

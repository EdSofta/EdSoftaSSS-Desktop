using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Repositories
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class SchemaInfo
    {
        public string Id { get; set; }
        public int Version { get; set; }
    }
}

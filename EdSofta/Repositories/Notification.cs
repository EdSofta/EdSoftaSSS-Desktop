using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Repositories
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class Notification
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string ExtraText { get; set; }
        public string Data { get; set; }
        public DateTime Date { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Repositories
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class Game
    {
        public Guid Id { get; set; }
        public string Profession { get; set; }
        public int Level { get; set; }
        public int Coins { get; set; }
        public bool Current { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastPlayed { get; set; }
        public Guid UserId { get; set; }
    }
}

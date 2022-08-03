using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Repositories;

namespace EdSofta.Data
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<User> Users { get; set; }
        public  DbSet<Notification> Notifications { get; set; }
        public DbSet<LearningRecommendation> LearningRecommendations { get; set; }
        public DbSet<Game> Games { get; set; }
    }
}

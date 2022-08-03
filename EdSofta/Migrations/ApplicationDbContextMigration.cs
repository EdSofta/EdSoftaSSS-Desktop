using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Repositories;

namespace EdSofta.Migrations
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class ApplicationDbContextMigration : DbContext
    {
        
        public static int RequiredDatabaseVersion = Convert.ToInt32(ViewModels.Utility.App.Variables["RequiredDbVersion"]);
       
        public DbSet<SchemaInfo> SchemaInfoes { get; set; }

        public void Initialize()
        {
            using (ApplicationDbContextMigration appDbContext = new ApplicationDbContextMigration())
            {
                var currentVersion = 0;
                if (appDbContext.SchemaInfoes.Any())
                    currentVersion = appDbContext.SchemaInfoes.Max(x => x.Version);
                var mmSqliteHelper = new ApplicationDbContextHelper();
                while (currentVersion < RequiredDatabaseVersion)
                {
                    currentVersion++;
                    foreach (string migration in mmSqliteHelper.Migrations[currentVersion])
                    {
                        appDbContext.Database.ExecuteSqlCommand(migration);
                    }
                    appDbContext.SchemaInfoes.Add(new SchemaInfo() { Version = currentVersion, Id = $"_dbMigration{currentVersion}" });
                    appDbContext.SaveChanges();
                }
            }

        }
    }
}

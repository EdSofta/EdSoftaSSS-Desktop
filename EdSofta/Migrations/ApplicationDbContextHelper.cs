using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EdSofta.ViewModels.Utility;

namespace EdSofta.Migrations
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class ApplicationDbContextHelper
    {
        public ApplicationDbContextHelper()
        {
            Migrations = new Dictionary<int, IList>();

            MigrationVersion1();
        }

        public Dictionary<int, IList> Migrations { get; set; }

        private void MigrationVersion1()
        {
            IList steps = new List<string>();
            var path = Path.Combine(ViewModels.Utility.App.AppDataPath,
                ViewModels.Utility.App.ResourcePaths["DbMigrations"], "_dbMigration1.sql");
            steps.Add(FileParser.readFile(path));

            Migrations.Add(1, steps);
        }

    }
}

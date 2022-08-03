using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Repositories;

namespace EdSofta.DataAccess
{
    public class GameRepository : BaseRepository<Game>
    {
        public GameRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Repositories;

namespace EdSofta.DataAccess
{
    public class QuestionRepository : BaseRepository<Question>
    {
        public QuestionRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}

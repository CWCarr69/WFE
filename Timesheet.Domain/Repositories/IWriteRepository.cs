using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Domain.Repositories
{
    public interface IWriteRepository<TEntity> where TEntity : Entity
    {
        Task Add(TEntity entity);
        void Delete(string id);
    }
}

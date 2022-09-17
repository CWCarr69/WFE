using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task<bool> CompleteAsync(CancellationToken token);
    }
}

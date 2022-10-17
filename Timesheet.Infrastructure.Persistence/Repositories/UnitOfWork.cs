using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Domain.Repositories;

namespace Timesheet.Infrastructure.Persistence.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly TimesheetDbContext _context;

        public UnitOfWork(TimesheetDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CompleteAsync(CancellationToken token)
        {
            var done = await _context.SaveChangesAsync(token);
            return done != 0;
        }
    }
}

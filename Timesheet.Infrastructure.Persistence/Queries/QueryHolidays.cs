using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Application.Queries;
using Timesheet.Domain.Models;

namespace Timesheet.Infrastructure.Persistence.Queries
{
    internal class QueryHolidays : ReadRepository<Holiday>, IHolidayQuery
    {
        public QueryHolidays(TimesheetDbContext context) : base(context)
        {
        }

        public Holiday? GetByDate(DateTime date) => _context.Holidays.SingleOrDefault(h => h.Date == date);

        public IEnumerable<Holiday> GetAllHolidays(DateTime? start = null, DateTime? end = null)
        {
            if(start is null && end is null)
            {
                return Get();
            }

            if(end is null)
            {
                return _context.Holidays.Where(h => h.Date >= start).AsEnumerable();
            }

            if(start is null)
            {
                return _context.Holidays.Where(h => h.Date <= end).AsEnumerable();
            }

            return _context.Holidays.Where(h => h.Date >= start && h.Date <= end).AsEnumerable();
        }

        public Task<IEnumerable<Holiday>> GetAllHolidays(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}

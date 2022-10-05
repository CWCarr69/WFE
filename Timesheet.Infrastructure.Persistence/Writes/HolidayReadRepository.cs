using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Repositories;

namespace Timesheet.Infrastructure.Persistence.Writes
{
    internal class HolidayReadRepository : ReadRepository<Holiday>, IHolidayReadRepository
    {
        public HolidayReadRepository(TimesheetDbContext context) : base(context)
        {
        }

        public Holiday? GetByDate(DateTime date)
        {
            return _context.Holidays.ToList().FirstOrDefault(e => e.Date.ToShortDateString == date.ToShortDateString);
        }
    }
}

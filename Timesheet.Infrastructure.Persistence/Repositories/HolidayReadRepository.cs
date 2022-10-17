using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Repositories;

namespace Timesheet.Infrastructure.Persistence.Repositories
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

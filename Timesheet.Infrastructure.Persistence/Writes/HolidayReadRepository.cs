using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
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
            var holiday = _context.Holidays.FirstOrDefault(e => e.Date.ToShortDateString == date.ToShortDateString);
            if(holiday == null)
            {
                throw new HolidayAlreadyExistException(date);
            }
            return holiday;
        }
    }
}

using Timesheet.Domain.Models;
using Timesheet.ReadModel.Queries;
using Timesheet.ReadModel.ReadModels;

namespace Timesheet.Infrastructure.Persistence.Queries
{
    internal class QueryHolidays : IQueryHoliday
    {
        private readonly TimesheetDbContext _context;

        public QueryHolidays(TimesheetDbContext context)
        {
            this._context = context;
        }

        public HolidayDetails? GetDetails(string id)
        {
            var holiday = _context.Holidays.Find(id);
            if (holiday == null)
            {
                return null;
            }
            return ToHolidayDetails(holiday);
        }

        public HolidayDetails? GetByDate(DateTime date)
        {
            var holiday = _context.Holidays.SingleOrDefault(h => h.Date == date);
            return ToHolidayDetails(holiday);
        }

        public IEnumerable<HolidayDetails> GetAllHolidays(DateTime? start = null, DateTime? end = null)
        {
            var holidays = Enumerable.Empty<Holiday>();

            if (start is null && end is null)
            {
                holidays = _context.Holidays;
            }else if (end is null)
            {
                holidays = _context.Holidays.Where(h => h.Date >= start).AsEnumerable();
            }else if (start is null)
            {
                holidays = _context.Holidays.Where(h => h.Date <= end).AsEnumerable();
            }
            else
            {
                holidays = _context.Holidays.Where(h => h.Date >= start && h.Date <= end).AsEnumerable();
            }

            return holidays.Select(holiday => ToHolidayDetails(holiday));
        }

        private HolidayDetails ToHolidayDetails(Holiday holiday) => new HolidayDetails
        {
            Date = holiday.Date,
            Description = holiday.Description,
            Notes = holiday.Notes,
            IsRecurrent = holiday.IsRecurrent,
        };
    }
}

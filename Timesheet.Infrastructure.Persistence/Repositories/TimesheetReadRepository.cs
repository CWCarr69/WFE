using Microsoft.EntityFrameworkCore;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Infrastructure.Persistence.Repositories
{
    internal class TimesheetReadRepository : ReadRepository<TimesheetHeader>, ITimesheetReadRepository
    {
        private readonly TimesheetDbContext _context;
        public TimesheetReadRepository(TimesheetDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TimesheetHeader?> GetTimesheet(string timesheetId)
        {
            return await _context.Timesheets.FindAsync(timesheetId);
        }

        public Task<TimesheetHeader?> GetTimesheetWithEntries(string timesheetId, string? employeeId = null)
        {
            if (employeeId is not null)
            {
                return _context.Timesheets.Where(t => t.Id == timesheetId)
                    .Include(t => t.TimesheetEntries.Where(te => te.EmployeeId == employeeId))
                    .Include(t => t.TimesheetHolidays)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return _context.Timesheets.Where(t => t.Id == timesheetId)
                     .Include(t => t.TimesheetEntries)
                     .Include(t => t.TimesheetHolidays)
                     .FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<TimesheetHeader?>> GetTimesheetByDate(DateTime date)
        {
            return await _context.Timesheets.Where(t => t.StartDate <= date && date <= t.EndDate)
                .Include(t => t.TimesheetEntries)
                .Include(t => t.TimesheetHolidays)
                .ToListAsync();
        }

        public async Task<TimesheetHeader?> GetTimesheetByDate(DateTime date, TimesheetType type)
        {
            return await _context.Timesheets
                .Where(t => t.StartDate <= date && date <= t.EndDate && t.Type == type)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TimesheetHeader?>> GetTimesheetByHoliday(string id)
        {
            return await _context.Timesheets
                .Where(t => t.TimesheetHolidays.Any(h => h.Id == id))
                .Include(t => t.TimesheetHolidays)
                .ToListAsync();
        }

        public async Task<bool> DoesEntryExists(string entryId)
        {
            return (await _context.Timesheets
                .Where(t => t.TimesheetEntries.Any(t => t.Id == entryId))
                .FirstOrDefaultAsync()) != null;
        }
    }
}
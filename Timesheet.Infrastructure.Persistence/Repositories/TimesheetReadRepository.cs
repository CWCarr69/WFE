using Microsoft.EntityFrameworkCore;
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

        public Task<TimesheetHeader?> GetFullTimesheet(string timesheetId, string? employeeId = null)
        {
            IQueryable timesheet = _context.Timesheets.Where(t => t.Id == timesheetId)
                .Include(t => t.TimesheetEntries);

            if (employeeId is not null)
            {
                return _context.Timesheets.Where(t => t.Id == timesheetId)
                    .Include(t => t.TimesheetEntries.Where(te => te.EmployeeId == employeeId))
                    .FirstOrDefaultAsync();
            }
            else
            {
                return _context.Timesheets.Where(t => t.Id == timesheetId)
                     .Include(t => t.TimesheetEntries)
                     .FirstOrDefaultAsync();
            }
        }
    }
}
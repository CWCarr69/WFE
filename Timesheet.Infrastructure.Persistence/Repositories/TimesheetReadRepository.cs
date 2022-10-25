﻿using Microsoft.EntityFrameworkCore;
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

        public Task<TimesheetHeader?> GetTimesheet(string timesheetId, string? employeeId = null)
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

        public async Task<IEnumerable<TimesheetHeader?>> GetTimesheetByDate(DateTime date)
        {
            return await _context.Timesheets.Where(t => t.StartDate >= date && t.EndDate <= date)
                .Include(t => t.TimesheetEntries)
                .Include(t => t.TimesheetHolidays)
                .ToListAsync();
        }

        public async Task<TimesheetHeader?> GetTimesheetByDate(DateTime date, TimesheetType type)
        {
            return await _context.Timesheets
                .Where(t => t.StartDate >= date && t.EndDate <= date && t.Type == type)
                .Include(t => t.TimesheetEntries)
                .Include(t => t.TimesheetHolidays)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TimesheetHeader?>> GetTimesheetByHoliday(string holidayId)
        {
            return await _context.Timesheets
                .Where(t => t.TimesheetHolidays.Any(h => h.Id == holidayId))
                .Include(t => t.TimesheetHolidays)
                .ToListAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Infrastructure.Persistence.Repositories
{
    internal class EmployeeReadRepository : ReadRepository<Employee>, IEmployeeReadRepository
    {
        public EmployeeReadRepository(TimesheetDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Employee>> GetAdministrators()
        {
            return await _context.Employees
                .Where(e => e.EmploymentData.IsAdministrator)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployee(string id)
        {
            return await _context.Employees
                .Include(e => e.PrimaryApprover)
                .Include(e => e.SecondaryApprover)
                .Include(e => e.Manager)
                .Include(e => e.Timeoffs)
                .ThenInclude(t => t.TimeoffEntries)
                .FirstOrDefaultAsync(e =>  e.Id == id);
        }


        public async Task<IDictionary<string, string>> GetEmployeesDictionary()
        {
            return await _context.Employees
                .ToDictionaryAsync(e => e.Id, e => e.FullName);
        }

        public async Task<IEnumerable<Employee>> GetEmployeeWithTimeoffsInPeriod(DateTime startDate, DateTime endDate)
        {
            return await _context.Employees
                .Where(e => e.Timeoffs.Any(
                    t => t.TimeoffEntries.Any(e => e.RequestDate >= startDate && e.RequestDate <= endDate)))
                .Include(e => e.Timeoffs)
                .ThenInclude(t => t.TimeoffEntries)
                .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Repositories;

namespace Timesheet.Infrastructure.Persistence.Repositories
{
    internal class EmployeeReadRepository : ReadRepository<Employee>, IEmployeeReadRepository
    {
        public EmployeeReadRepository(TimesheetDbContext context) : base(context)
        {
        }

        public Task<IEnumerable<Employee>> GetAdministrators()
        {
            throw new NotImplementedException();
        }

        public Holiday? GetByDate(DateTime date)
        {
            return _context.Holidays.ToList().FirstOrDefault(e => e.Date.ToShortDateString == date.ToShortDateString);
        }

        public Task<Employee?> GetEmployee(string id)
        {
            return _context.Employees
                .Include(e => e.PrimaryApprover)
                .Include(e => e.SecondaryApprover)
                .Include(e => e.Manager)
                .Include(e => e.Timeoffs)
                .ThenInclude(t => t.TimeoffEntries)
                .FirstOrDefaultAsync(e =>  e.Id == id);
        }
    }
}

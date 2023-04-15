using Microsoft.EntityFrameworkCore;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Repositories;
using static Dapper.SqlMapper;

namespace Timesheet.Infrastructure.Persistence.Repositories
{
    internal class HierarchyRepository : IHierarchyRepository
    {
        protected readonly TimesheetDbContext _context;

        public HierarchyRepository(TimesheetDbContext context)
        {
            this._context = context;
        }

        public async Task<bool> IsEmployeeManager(string employeeId, string managerId)
        {
            return await this._context.EmployeeHierarchy
                .AsQueryable()
                .AnyAsync(e => e.EmployeeId == employeeId && e.ManagerId == managerId);
        }
    }
}

using Timesheet.Application.Employees.Queries;
using Timesheet.Domain.Models;

namespace Timesheet.Infrastructure.Persistence.Queries
{
    internal class QueryEmployee : ReadRepository<Employee>, IQueryEmployee
    {
        public QueryEmployee(TimesheetDbContext context) : base(context)
        {
        }

        public Employee? GetEmployeeProfile(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Employee> GetEmployeesWithTimeRecordStatus(string managerId = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Employee> GetEmployeesWithPendingTimeoffs(string managerId = null)
        {
            throw new NotImplementedException();
        }
    }
}

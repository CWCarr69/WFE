using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.Queries
{
    public interface IQueryEmployee : IReadRepository<Employee>
    {
        Employee? GetEmployeeProfile(string id);
        IEnumerable<Employee> GetEmployeesWithTimeRecordStatus(string managerId=null);
        public IEnumerable<Employee> GetEmployeesWithPendingTimeoffs(string managerId=null);

    }
}

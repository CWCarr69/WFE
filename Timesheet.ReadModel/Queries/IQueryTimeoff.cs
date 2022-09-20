using Timesheet.Domain.Models;

namespace Timesheet.ReadModel.Queries
{
    public interface IQueryTimeoff
    {
        Employee? GetEmployeeTimeoffWithEntries(string employeeId, string timeoffId);
        Employee? GetEmployeeTimeoff(string employeeId, string timeoffId);
        Employee? GetEmployeeTimeoffs(string employeeId, DateTime? startDate = null, DateTime? endDate = null);

        IEnumerable<Employee?> GetEmployeePendingTimeoffs (string approverId);
    }
}

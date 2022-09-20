using Timesheet.Domain.Models;
using Timesheet.ReadModel.Queries;

namespace Timesheet.Infrastructure.Persistence.Queries
{
    internal class QueryTimeoff : IQueryTimeoff
    {
        public IEnumerable<Employee?> GetEmployeePendingTimeoffs(string approverId)
        {
            throw new NotImplementedException();
        }

        public Employee? GetEmployeeTimeoff(string employeeId, string timeoffId)
        {
            throw new NotImplementedException();
        }

        public Employee? GetEmployeeTimeoffs(string employeeId, DateTime? startDate = null, DateTime? endDate = null)
        {
            throw new NotImplementedException();
        }

        public Employee? GetEmployeeTimeoffWithEntries(string employeeId, string timeoffId)
        {
            throw new NotImplementedException();
        }
    }
}

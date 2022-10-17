using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Application.Employees.Queries
{
    public interface IQueryTimeoff
    {
        Task<EmployeeTimeoff> GetEmployeeTimeoffDetails(string employeeId, string timeoffId);
        Task<IEnumerable<EmployeeTimeoffDetailSummary?>> GetEmployeeTimeoffSummary(string employeeId, string timeoffId);
        Task<IEnumerable<EmployeeTimeoff>> GetEmployeeTimeoffs(string employeeId);
        Task<IEnumerable<EmployeeTimeoffMonthStatistics>> GetEmployeeTimeoffsMonthStatistics(string employeeId);
    }
}

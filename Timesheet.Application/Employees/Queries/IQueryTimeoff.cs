using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Application.Employees.Queries
{
    public interface IQueryTimeoff
    {
        Task<IEnumerable<EmployeeTimeoffDetail>> GetEmployeeTimeoffDetails(string employeeId, string timeoffId);
        Task<EmployeeTimeoffDetailSummary?> GetEmployeeTimeoffSummary(string employeeId, string timeoffId);
        Task<IEnumerable<EmployeeTimeoff>> GetEmployeeTimeoffs(string employeeId);
        Task<EmployeeTimeoffMonthStatistics> GetEmployeeTimeoffsMonthStatistics(string employeeId);
    }
}

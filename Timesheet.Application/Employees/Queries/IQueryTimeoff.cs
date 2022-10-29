using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Application.Employees.Queries
{
    public interface IQueryTimeoff
    {
        Task<EmployeeTimeoff> GetEmployeeTimeoffDetails(string employeeId, string timeoffId);
        Task<IEnumerable<EmployeeTimeoffDetailSummary?>> GetEmployeeTimeoffSummary(string employeeId, string timeoffId);
        Task<EmployeeTimeoffHistory> GetEmployeeTimeoffs(string employeeId, int page, int itemsPerPage);
        Task<IEnumerable<EmployeeTimeoffMonthStatistics>> GetEmployeeTimeoffsMonthStatistics(string employeeId);
    }
}

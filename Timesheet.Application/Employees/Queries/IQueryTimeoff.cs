using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Application.Employees.Queries
{
    public interface IQueryTimeoff
    {
        Task<EmployeeTimeoff> GetEmployeeTimeoffDetails(string employeeId, string timeoffId);
        Task<IEnumerable<EmployeeTimeoffDetailSummary?>> GetEmployeeTimeoffSummary(string employeeId, string timeoffId);
        Task<EmployeeTimeoffHistory> GetEmployeeTimeoffs(string employeeId, int page, int itemsPerPage/*, bool requireApproval*/);
        Task<IEnumerable<EmployeeTimeoffMonthStatisticsGroupByMonth>> GetEmployeeTimeoffsMonthStatistics(string employeeId);
        Task<IEnumerable<EmployeeTimeoffEntry>> GetEmployeeTimeoffEntriesInPeriod(string employeeId, DateTime start, DateTime end/*, bool requireApproval*/);
    }
}

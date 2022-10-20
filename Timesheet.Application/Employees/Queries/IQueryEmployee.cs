using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Employees.Queries
{
    public interface IQueryEmployee
    {
        Task<IEnumerable<EmployeeProfile?>> GetEmployees();
        Task<EmployeeProfile?> GetEmployeeProfile(string id);
        Task<EmployeeProfile?> GetEmployeeProfileByEmail(string email);
        Task<EmployeeProfile?> GetEmployeeProfileByLogin(string login);
        Task<EmployeeApprovers?> GetEmployeeApprovers(string id);
        Task<EmployeeBenefits?> GetEmployeeBenefits(string id);
        Task<IEnumerable<EmployeeWithTimeStatus>> GetEmployeesTimeRecordStatus(string? approverId = null, bool directReports = false);
        Task<IEnumerable<EmployeeTimeoff>> GetEmployeesPendingTimeoffs(string? approverId = null, bool directReports = false);
        Task<IEnumerable<EmployeeTimesheet>> GetEmployeesPendingTimesheets(string? approverId = null, bool directReports = false);
        Task<double> CalculateUsedBenefits(string employeeId, Domain.Models.Employees.TimeoffType type, DateTime start, DateTime end);
        Task<double> CalculateScheduledBenefits(string employeeId, Domain.Models.Employees.TimeoffType type, DateTime start, DateTime end);
    }
}

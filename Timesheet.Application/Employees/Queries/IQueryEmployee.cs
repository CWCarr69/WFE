using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.ReadModels.Employees;

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
        Task<EmployeeTeam> GetEmployeeTeam(int page, int itemsPerPage, string? approverId = null, bool directReports = false);
        Task<EmployeePendingTimeoffs> GetEmployeesPendingTimeoffs(int page, int itemsPerPage, string? approverId = null, bool directReports = false);
        Task<EmployeePendingTimesheets> GetEmployeesPendingTimesheets(int page, int itemsPerPage, string? approverId = null, bool directReports = false);
        Task<double> CalculateUsedBenefits(string employeeId, TimeoffType type, DateTime start, DateTime end);
        Task<double> CalculateScheduledBenefits(string employeeId, TimeoffType type);
    }
}

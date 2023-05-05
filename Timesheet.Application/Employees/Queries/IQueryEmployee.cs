using Timesheet.Domain.ReadModels.Employees;
using EmployeeBenefits = Timesheet.Domain.ReadModels.Employees.EmployeeBenefits;

namespace Timesheet.Application.Employees.Queries
{
    public interface IQueryEmployee
    {
        Task<IEnumerable<EmployeeProfile?>> GetEmployees();
        Task<EmployeeProfile?> GetEmployeeProfile(string id, bool withApprovers=false);
        Task<EmployeeProfile?> GetEmployeeProfileByEmail(string email);
        Task<EmployeeProfile?> GetEmployeeProfileByLogin(string login);
        Task<EmployeeApprovers?> GetEmployeeApprovers(string id);
        Task<EmployeeBenefits?> GetEmployeeBenefitsVariation(string id);
        Task<EmployeeTeam> GetEmployeeTeam(int page, int itemsPerPage, string? approverId = null, bool directReports = false);
        Task<EmployeePendingTimeoffs> GetEmployeesPendingTimeoffs(int page, int itemsPerPage, string? approverId = null, bool directReports = false);
        Task<EmployeePendingTimesheets> GetEmployeesPendingTimesheets(int page, int itemsPerPage, string? approverId = null, bool directReports = false);
        Task<EmployeeOrphanTimesheets> GetEmployeesOrphanTimesheets(int page, int itemsPerPage, string? approverId = null, bool directReports = false);
        Task<double> CalculateUsedBenefits(string employeeId, int type, DateTime start, DateTime end);
        Task<double> CalculateScheduledBenefits(string employeeId, int type);
    }
}

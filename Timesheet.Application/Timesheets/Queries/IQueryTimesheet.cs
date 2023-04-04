using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Queries
{
    public interface IQueryTimesheet
    {

        Task<EmployeeTimesheet?> GetEmployeeTimesheetDetails(string employeeId, string timesheetId);
        Task<IEnumerable<EmployeeTimesheetDetailSummary?>> GetEmployeeTimesheetSummaryByPayrollCode(string employeeId, string timesheetId);
        Task<IEnumerable<EmployeeTimesheetDetailSummary?>> GetEmployeeTimesheetSummaryByDate(string employeeId, string timesheetId);
        Task<EmployeeTimesheetHistory> GetEmployeeTimesheets(string employeeId, int page, int itemsPerPage);
        Task<TimesheetReview> GetTimesheetReview(string payrollPeriod, string? employeeId, string? department, int page, int itemsPerPage, string managerId = null);
        Task<AllEmployeesTimesheet<TimesheetEntryDetails>> GetAllTimesheetEntriesByPayrollPeriodAndCriteria(string payrollPeriod, string? department, string? employeeId);
        Task<AllEmployeesTimesheet<ExternalTimesheetEntryDetails>> GetAllTimesheetEntriesByPayrollPeriod(string payrollPeriod, bool ignoreHolidays);
        Task<IEnumerable<EmployeeTimesheetEntry>> GetEmployeeTimesheetEntriesInPeriod(string employeeId, DateTime start, DateTime end);
    }
}

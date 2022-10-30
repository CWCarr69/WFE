using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Queries
{
    public interface IQueryTimesheet
    {

        Task<EmployeeTimesheet?> GetEmployeeTimesheetDetails(string employeeId, string timesheetId);
        Task<IEnumerable<EmployeeTimesheetDetailSummary?>> GetEmployeeTimesheetSummaryByPayrollCode(string employeeId, string timesheetId);
        Task<IEnumerable<EmployeeTimesheetDetailSummary?>> GetEmployeeTimesheetSummaryByDate(string employeeId, string timesheetId);
        Task<EmployeeTimesheetHistory> GetEmployeeTimesheets(string employeeId, int page, int itemsPerPage);
        Task<TimesheetReview> GetTimesheetReview(string payrollPeriod, string? employeeId, string? department, int page, int itemsPerPage);
        Task<AllEmployeesTimesheet?> GetAllEmployeeTimesheetByPayrollPeriod(string payrollPeriod);
    }
}

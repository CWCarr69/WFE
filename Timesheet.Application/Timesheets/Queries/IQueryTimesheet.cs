using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Queries
{
    public interface IQueryTimesheet
    {

        Task<EmployeeTimesheet?> GetEmployeeTimesheetDetails(string employeeId, string timesheetId);
        Task<IEnumerable<EmployeeTimesheetDetailSummary?>> GetEmployeeTimesheetSummaryByPayroll(string employeeId, string timesheetId);
        Task<IEnumerable<EmployeeTimesheetDetailSummary?>> GetEmployeeTimesheetSummaryByDate(string employeeId, string timesheetId);
        Task<IEnumerable<EmployeeTimesheet>> GetEmployeeTimesheets(string employeeId);
        Task<TimesheetReview> GetTimesheetReview(string payrollPeriod, string? employeeId, string? department, int page, int itemsPerPage);
    }
}

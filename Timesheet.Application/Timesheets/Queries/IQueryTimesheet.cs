using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Queries
{
    public interface IQueryTimesheet
    {

        Task<EmployeeTimesheetEntry?> GetEmployeeTimesheetDetails(string timesheetId);
        Task<EmployeeTimesheetDetailSummary?> GetEmployeeTimeoffSummary(string timesheetId);
        Task<IEnumerable<EmployeeTimesheet>> GetEmployeeTimesheets(string employeeId);
        Task<IEnumerable<EmployeeTimesheet>> GetEmployeeTimesheetReview(string? payrollPeriod, string? employeeId, string? department, IEnumerable<TimesheetStatus> statuses);
    }
}

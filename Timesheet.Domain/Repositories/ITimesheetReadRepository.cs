using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.Repositories
{
    public interface ITimesheetReadRepository : IReadRepository<TimesheetHeader>
    {
        Task<TimesheetHeader?> GetTimesheet(string timesheetId, string? employeeId=null);
        Task<TimesheetHeader?> GetTimesheetByPayrollPeriod(string payrollPeriod);
        Task<IEnumerable<TimesheetHeader?>> GetTimesheetByDate(DateTime date);
        Task<TimesheetHeader> GetTimesheetByDate(DateTime date, TimesheetType type);
        Task<IEnumerable<TimesheetHeader?>> GetTimesheetByHoliday(string holidayId);
    }
}

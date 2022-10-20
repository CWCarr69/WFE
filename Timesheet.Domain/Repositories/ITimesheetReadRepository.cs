using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.Repositories
{
    public interface ITimesheetReadRepository : IReadRepository<TimesheetHeader>
    {
        Task<TimesheetHeader?> GetFullTimesheet(string timesheetId, string? employeeId=null);
    }
}

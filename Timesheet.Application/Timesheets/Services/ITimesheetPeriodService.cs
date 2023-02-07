using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Services
{
    public interface ITimesheetPeriodService
    {
        TimesheetPeriod GetCurrentPeriod(bool isSalaried);
    }
}

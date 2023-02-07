using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Services
{
    public class TimesheetPeriodService : ITimesheetPeriodService
    {
        public TimesheetPeriod GetCurrentPeriod(bool isSalaried)
        {
            var timesheet = isSalaried
                ? TimesheetHeader.CreateMonthlyTimesheet(DateTime.Now)
                : TimesheetHeader.CreateWeeklyTimesheet(DateTime.Now);

            return new TimesheetPeriod
            {
                Start = timesheet.StartDate,
                End = timesheet.EndDate
            };
        }
    }
}

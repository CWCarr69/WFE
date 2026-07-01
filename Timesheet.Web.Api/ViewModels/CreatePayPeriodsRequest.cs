using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Web.Api.ViewModels
{
    public class CreatePayPeriodsRequest
    {
        public DateTime StartDate { get; set; }
        public TimesheetType Type { get; set; } = TimesheetType.WEEKLY;
    }
}
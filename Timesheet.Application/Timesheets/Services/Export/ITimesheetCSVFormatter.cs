namespace Timesheet.Application.Timesheets.Services.Export
{
    public interface ITimesheetCSVFormatter
    {
        string Format(TimesheetCSVModel timesheet);
    }
}

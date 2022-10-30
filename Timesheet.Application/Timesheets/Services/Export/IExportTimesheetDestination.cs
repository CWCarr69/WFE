namespace Timesheet.Application.Timesheets.Services.Export
{
    public interface IExportTimesheetDestination
    {
        string? GetPath();
    }
}
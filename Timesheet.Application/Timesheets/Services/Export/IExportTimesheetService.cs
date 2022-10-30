namespace Timesheet.Application.Timesheets.Services.Export
{
    public interface IExportTimesheetService
    {
        Task<string> ExportToWeb(string payrollperiod);
        Task ExportToExternal(string payrollperiod);
    }
}

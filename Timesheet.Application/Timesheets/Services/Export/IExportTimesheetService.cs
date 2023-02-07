namespace Timesheet.Application.Timesheets.Services.Export
{
    public interface IExportTimesheetService
    {
        Task<string> ExportRawReviewToWeb(string payrollperiod);
        Task<string> ExportAdaptedReviewToWeb(string payrollperiod);
        Task ExportAdaptedReviewToExternal(string payrollperiod);
    }
}

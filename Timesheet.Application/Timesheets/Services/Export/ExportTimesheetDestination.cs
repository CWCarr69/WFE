namespace Timesheet.Application.Timesheets.Services.Export
{
    internal class ExportTimesheetDestination : IExportTimesheetDestination
    {
        private readonly string path;

        public ExportTimesheetDestination(string path)
        {
            this.path = path;
        }

        public string? GetPath() => path;
    }
}

namespace Timesheet.Application.Timesheets.Services.Export
{
    public interface ITimesheetCSVWriter
    {
        Task Write(string csv);
        void SetPath(string path);
    }
}

namespace Timesheet.Application.Timesheets.Services.Export
{
    public class TimesheetCSVModel<T>
    {
        public List<T> Entries { get; set; } = new List<T> ();
    }
}

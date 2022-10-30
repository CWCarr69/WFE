namespace Timesheet.Application.Timesheets.Services.Export
{
    public class TimesheetCSVModel
    {
        public List<TimesheetCSVEntryModel> Entries { get; set; } = new List<TimesheetCSVEntryModel> ();
    }
}

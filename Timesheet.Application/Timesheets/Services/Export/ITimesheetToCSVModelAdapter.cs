using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Services.Export
{
    public interface ITimesheetToCSVModelAdapter<TEntry, TEntryCsv>
    {
        TimesheetCSVModel<TEntryCsv> Adapt(AllEmployeesTimesheet<TEntry> timesheet);
    }
}

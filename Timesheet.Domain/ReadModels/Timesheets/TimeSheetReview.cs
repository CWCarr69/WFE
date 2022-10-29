namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class TimesheetReview : WithTotal<EmployeeTimesheetWithTotals>
    {
        public double TotalQuantity { get; set; }
        public IEnumerable<EmployeeTimesheetWithTotals> Items { get; set; }
    }
}

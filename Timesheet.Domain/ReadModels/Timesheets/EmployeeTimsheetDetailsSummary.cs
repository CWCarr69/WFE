namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class EmployeeTimesheetDetailSummary
    {
        public DateTime? WorkDate { get; set; }
        public int? PayrollCodeId { get; set; }
        public string? PayrollCode { get; set; }
        public double Hours { get; set; }
    }
}

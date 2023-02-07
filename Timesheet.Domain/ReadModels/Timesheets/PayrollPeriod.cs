namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class PayrollPeriod
    {
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

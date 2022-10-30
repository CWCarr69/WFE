namespace Timesheet.Application.Timesheets.Services.Export
{
    public class TimesheetCSVEntryModel
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string WorkDate { get; set; }
        public string PayrollCode { get; set; }
        public double Quantity { get; set; }
        public string Description { get; set; }
        public string ServiceOrder { get; set; } = String.Empty;
        public string ServiceOrderDescription { get; set; } = String.Empty;
        public string Job { get; set; } = String.Empty;
        public string JobDescription { get; set; } = String.Empty;
        public string JobTask { get; set; } = String.Empty;
        public string LaborCode { get; set; } = String.Empty;
        public string Customer { get; set; } = String.Empty;
        public string ProfitCenter { get; set; } = String.Empty;
        public string WorkArea { get; set; } = String.Empty;
    }
}
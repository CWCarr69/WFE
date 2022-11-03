using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class EmployeeTimesheetEntry
    {
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime WorkDate { get; set; }
        public string PayrollCode { get; set; }
        public double Quantity { get; set; }
        public string CustomerNumber { get; set; }
        public string JobNumber { get; set; }
        public string JobDescription { get; set; }
        public string JobTaskNumber { get; set; }
        public string JobTaskDescription { get; set; }
        public string LaborCode { get; set; }
        public string ServiceOrderNumber { get; set; }
        public string ServiceOrderDescription { get; set; }
        public string ProfitCenterNumber { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public bool OutOffCountry { get; set; }
        public string WorkArea => OutOffCountry ? "Out of country" : "In state";
        public TimesheetEntryStatus Status { get; set; }
        public string StatusName => Status.ToString();
    }
}

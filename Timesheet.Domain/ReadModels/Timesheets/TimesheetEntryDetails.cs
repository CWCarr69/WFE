using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class ExternalTimesheetEntryDetails
    {
        public string No { get; set; }
        public string DetCode { get; set; }
        public double Hours { get; set; }
        public string CC1 { get; set; }
        public string Job_Code { get; set; }
        public string Begin_Date { get; set; }
        public string End_Date { get; set; }
        public int Sequence => 1;
    }

    public class TimesheetEntryDetails
    {
        public string TimesheetId { get; set; }
        public string EmployeeId { get; set; }
        public string Fullname { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PayrollPeriod { get; set; }
        public double Total { get; set; }
        public double Overtime { get; set; }
        public TimesheetStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string TimesheetEntryId { get; set; }
        public DateTime WorkDate { get; set; }
        public string PayrollCode { get; set; }
        public double Quantity { get; set; }
        public string CustomerNumber { get; set; }
        public string JobNumber { get; private set; }
        public string JobDescription { get; private set; }
        public string JobTaskNumber { get; private set; }
        public string JobTaskDescription { get; private set; }
        public string LaborCode { get; private set; }
        public string ServiceOrderNumber { get; private set; }
        public string ServiceOrderDescription { get; private set; }
        public string ProfitCenterNumber { get; private set; }
        public string Department { get; private set; }
        public string Description { get; private set; }
        public bool OutOffCountry { get; private set; }
        public string WorkArea => OutOffCountry ? "Out of country" : "In state";
        public TimesheetEntryStatus TimesheetEntryStatus { get; set; }
        public string TimesheetEntryStatusName => TimesheetEntryStatus.ToString();
    }
}
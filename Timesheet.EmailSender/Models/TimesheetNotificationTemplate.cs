namespace Timesheet.EmailSender.Models
{
    public class TimesheetEntryRowTemplate
    {
        public string PayrollCode { get; set; }
        public decimal Quantity { get; set; }
        public string ServiceOrderNo { get; set; }
        public string CustomerName { get; set; }
        public string WorkDate { get; set; }
        public string ProfitCenter { get; set; }
        public string LaborCode { get; set; }
        public string JobTaskNo { get; set; }
        public string RelatedEmployeeId { get; set; }
    } 

    public class TimesheetNotificationTemplate : BaseNotificationTemplate
    {
        public TimesheetNotificationTemplate() { }
        public TimesheetNotificationTemplate(TimesheetNotificationTemplate timesheet) 
        {
            this.PayrollPeriod = timesheet.PayrollPeriod;
            this.PayrollStartDate = timesheet.PayrollStartDate;
            this.PayrollEndDate = timesheet.PayrollEndDate;
            this.EmployeeName = timesheet.EmployeeName;
            this.ManagerName= timesheet.ManagerName;
            this.EmployeeComment = timesheet.EmployeeComment;
            this.SupervisorComment = timesheet.SupervisorComment;
            this.Link = timesheet.Link;
        }

        public string PayrollPeriod { get; set; }
        public string PayrollStartDate { get; set; }
        public string PayrollEndDate { get; set; }
        public string Status { get; set; }
        public string EmployeeComment { get; set; }
        public string SupervisorComment { get; set; }
        public string Link { get; set; }
        public IEnumerable<TimesheetEntryRowTemplate> TimesheetEntries { get; set; }
        public override DateTime ReferenceDate => DateTime.Parse(TimesheetEntries?.OrderBy(r => r.WorkDate).FirstOrDefault().WorkDate);
        public decimal Total => TimesheetEntries.Sum(t => t.Quantity);
    }
}
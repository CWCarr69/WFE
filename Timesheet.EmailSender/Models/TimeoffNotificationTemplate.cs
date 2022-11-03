using Timesheet.Domain.Models.Employees;

namespace Timesheet.EmailSender.Models
{
    public class TimeoffEntryRowTemplate
    {
        public string Date { get; set; }
        public double Hours { get; set; }
        public TimeoffEntryStatus VacationTypeId { get; set; }
        public string VacationType => VacationTypeId.ToString();
    } 

    public class TimeoffNotificationTemplate : BaseNotificationTemplate
    {
        public string EmployeeName { get; set; }
        public string DateCreated { get; set; }
        public string Status { get; set; }
        public string ManagerName { get; set; }
        public string EmployeeComment { get; set; }
        public string SupervisorComment { get; set; }
        public IEnumerable<TimeoffEntryRowTemplate> Rows { get; set; }
        public double Total => Rows.Sum(r => r.Hours);
    }
}
using Timesheet.Domain.Models.Employees;

namespace Timesheet.EmailSender.Models
{
    public class TimeoffEntryRowTemplate
    {
        public string Date { get; set; }
        public double Hours { get; set; }
        public string VacationType { get; set; }
    } 

    public record TimeoffNotificationTemplate : BaseNotificationTemplate
    {
        public string DateCreated { get; set; }
        public string Status { get; set; }
        public string EmployeeComment { get; set; }
        public string SupervisorComment { get; set; }
        public IEnumerable<TimeoffEntryRowTemplate> Rows { get; set; }
        public override DateTime? ReferenceDate => Rows is not null && Rows.Any() ? DateTime.Parse(Rows.OrderBy(r => r.Date).FirstOrDefault().Date) : null;
        public double Total => Rows.Sum(r => r.Hours);
    }
}
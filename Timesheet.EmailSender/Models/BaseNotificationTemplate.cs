namespace Timesheet.EmailSender.Models
{
    public class BaseNotificationTemplate
    {
        public string NotificationId { get; set; }
        public string ItemId { get; set; }
        public string EmployeeId { get; set; }
        public string RelatedEmployeeId { get; set; }
        public string Subject { get; set; }
        public string Link { get; set; }
        public string EmployeeName { get; set; }
        public string ManagerName { get; set; }
        public virtual DateTime ReferenceDate { get; }

        public bool Sent { get; set; } = false;
        public void Complete() => Sent = true;
    }
}
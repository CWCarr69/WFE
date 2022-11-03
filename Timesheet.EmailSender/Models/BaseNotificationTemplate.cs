namespace Timesheet.EmailSender.Models
{
    public class BaseNotificationTemplate
    {
        public string NotificationId { get; set; }
        public string ItemId { get; set; }
        public string EmployeeId { get; set; }
        public string Subject { get; set; }
        public string ViewLink { get; set; }

        public bool Sent { get; set; } = false;
        public void Complete() => Sent = true;
    }
}
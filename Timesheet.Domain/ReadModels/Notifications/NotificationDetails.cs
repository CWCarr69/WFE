using Timesheet.Domain.Models.Notifications;

namespace Timesheet.Domain.ReadModels.Notifications
{
    public class NotificationDetails
    {
        public string Id { get; set; }
        public int Population { get; set; }
        public NotificationType Group { get; set; }
        public string Action { get; set; }
    }
}

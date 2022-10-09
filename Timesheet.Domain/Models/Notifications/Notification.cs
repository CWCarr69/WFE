namespace Timesheet.Domain.Models.Notifications
{
    public class Notification : Entity
    {
        public Notification(string id) : base(id)
        {
        }

        public int Population { get; private set; }
        public NotificationType Group { get; private set; }
        public string Action { get; private set; }
    }
}

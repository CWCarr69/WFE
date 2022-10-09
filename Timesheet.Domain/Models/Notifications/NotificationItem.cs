namespace Timesheet.Domain.Models.Notifications
{
    public class NotificationItem : Entity
    {
        public string EmployeeId { get; private set; }
        public string Action { get; private set; }
        public string Subject { get; private set; }
        public bool Sent { get; private set; }
        public string ObjectId { get; private set; }

        public NotificationItem(string id) : base(id)
        {
        }

        public static NotificationItem Create(string employeeId, string action, string subject, bool sent, string objectId)
        {
            var notification = new NotificationItem(Entity.GenerateId()) {
                EmployeeId = employeeId,
                Action = action,
                Subject = subject,
                Sent = sent,
                ObjectId = objectId
            };

            return notification;
        }
    }
}

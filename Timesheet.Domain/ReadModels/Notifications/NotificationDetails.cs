using System.Text.Json.Serialization;

namespace Timesheet.Domain.ReadModels.Notifications
{
    public class NotificationDetails
    {
        public string Id { get; set; }
        public int Population { get; set; }
    }
}

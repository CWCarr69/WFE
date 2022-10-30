using Timesheet.Domain.Models.Notifications;

namespace Timesheet.Web.Api.ViewModels
{
    public class NotificationDto
    {
        public string Id { get; set; }
        public NotificationType Group { get; set; }
        public string GroupName => Group.ToString();
        public string Action { get; set; }
        public IEnumerable<NotificationPopulationDto> Populations { get; set; }
    }
}

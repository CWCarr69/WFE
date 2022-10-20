using Timesheet.Application.Notifications.Commands;
using Timesheet.Application.Notifications.Services;
using Timesheet.Domain.Models.Notifications;

namespace Timesheet.Web.Api.ViewModels
{
    public class NotificationUpdateModel
    {
        public string Id { get; set; }
        public IEnumerable<NotificationPopulationType> Populations { get; set; }
    }
}

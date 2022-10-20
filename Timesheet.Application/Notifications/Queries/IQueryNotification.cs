using Timesheet.Domain.ReadModels.Notifications;

namespace Timesheet.Application.Notifications.Queries
{
    public interface IQueryNotification
    {
        Task<IEnumerable<NotificationDetails>> GetNotifications();
    }
}

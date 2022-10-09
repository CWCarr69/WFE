using Timesheet.Domain.Models.Notifications;

namespace Timesheet.Domain.Repositories
{
    public interface INotificationReadRepository : IReadRepository<Notification>
    {
        Notification? GetByGroupAndAction(NotificationType group, string action);
    }
}

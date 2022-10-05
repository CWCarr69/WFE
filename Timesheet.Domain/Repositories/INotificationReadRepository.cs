using Timesheet.Domain.Models;
using NotificationType = Timesheet.Domain.Models.SubDomainType;

namespace Timesheet.Domain.Repositories
{
    public interface INotificationReadRepository : IReadRepository<Notification>
    {
        Notification? GetByGroupAndAction(NotificationType group, string action);
    }
}

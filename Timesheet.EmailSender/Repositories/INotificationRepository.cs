
using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Repositories
{
    internal interface INotificationRepository
    {
        IEnumerable<NotificationItem> Get();
        void CompleteTransaction();
    }
}
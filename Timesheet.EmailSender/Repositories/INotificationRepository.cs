
using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Repositories
{
    internal interface INotificationRepository
    {
        void CompleteSend(IEnumerable<string> ids);
        IEnumerable<TimeoffNotificationTemplate> GetTimeoffNotifications();
        IEnumerable<TimesheetNotificationTemplate> GetTimesheetNotifications();
    }
}

using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Repositories
{
    internal interface INotificationRepository
    {
        void CompleteSend(params string[] ids);
        IEnumerable<TimeoffNotificationTemplate> GetTimeoffNotifications(bool includeSent = false, bool takeFirst = false);
        IEnumerable<TimesheetNotificationTemplate> GetTimesheetNotifications(bool includeSent = false, bool takeFirst = false);
        TimeoffNotificationTemplate GetTestTimeoffNotification();
        TimesheetNotificationTemplate GetTestTimesheetNotification();
    }
}
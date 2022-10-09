using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Services
{
    internal interface ITemplateProcessor
    {
        string ProcessNotification(NotificationItem notificationItem);
    }
}
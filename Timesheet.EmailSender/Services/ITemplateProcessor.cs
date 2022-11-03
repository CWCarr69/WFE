using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Services
{
    internal interface ITemplateProcessor
    {
        string ProcessNotification<T>(T notificationItem);
        void SetTemplate(string template);
    }
}
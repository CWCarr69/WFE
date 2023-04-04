using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Services
{
    internal interface ITemplateProcessor
    {

        public string GetWebAppUri();
        public string GetTemplatesBasePath();
        string ProcessNotification<T>(T notificationItem);
        void SetTemplate(string template);
    }
}
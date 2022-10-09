using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Services
{
    internal class MailEngine
    {
        private ITemplateProcessor _templateProcessor;
        private IMailSender _mailSender;

        internal MailEngine(ITemplateProcessor templateProcessor, IMailSender mailSender)
        {
            this._templateProcessor = templateProcessor;
            this._mailSender = mailSender;
        }

        internal void SendEmail(NotificationItem notificationItem)
        {
            var message = _templateProcessor.ProcessNotification(notificationItem);
            _mailSender.Send(message, notificationItem.EmployeeEmail, notificationItem.Subject);
        }
    }
}

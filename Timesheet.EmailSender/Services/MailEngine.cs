using System.Configuration;
using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Services
{
    internal class MailEngine
    {
        private readonly ITemplateProcessor _templateProcessor;
        private readonly IMailSender _mailSender;
        private readonly IDictionary<string, string> _employeeEmails;

        private readonly Uri _host = new Uri(ConfigurationManager.AppSettings.Get("host"));
        private readonly string _timeheetTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/TimesheetEmailTemplate.html");
        private readonly string _timeoffTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Templates/TimeoffEmailTemplate.html");
        private const string _timesheetViewLink = "/Timesheet/{0}/Employee/{1}";
        private const string _timeoffViewLink = "/Employee/{0}/Timeoff/{1}/";

        internal MailEngine(ITemplateProcessor templateProcessor, IMailSender mailSender, IDictionary<string, string> employeeEmails)
        {
            this._templateProcessor = templateProcessor;
            this._mailSender = mailSender;
            this._employeeEmails = employeeEmails;
        }

        internal void SendEmail<T>(T notificationItem)
            where T : BaseNotificationTemplate
        {
            if(notificationItem is TimeoffNotificationTemplate)
            {
                var linkFormat = new Uri(_host, _timeoffViewLink).ToString();
                notificationItem.ViewLink = string.Format(linkFormat, notificationItem.ItemId);
                SendEmail(notificationItem, _timeoffTemplatePath);
            }
            else
            {
                var linkFormat = new Uri(_host, _timesheetViewLink).ToString();
                notificationItem.ViewLink = string.Format(linkFormat, notificationItem.ItemId);
                SendEmail(notificationItem, _timeheetTemplatePath);
            }
        }

        private void SendEmail<T>(T notificationItem, string template)
            where T: BaseNotificationTemplate
        {
            _templateProcessor.SetTemplate(template);
            if (_employeeEmails.TryGetValue(notificationItem.EmployeeId, out var toEmail))
            {

                var message = _templateProcessor.ProcessNotification(notificationItem);
                _mailSender.Send(message, toEmail, notificationItem.Subject);
            }
        }
    }
}

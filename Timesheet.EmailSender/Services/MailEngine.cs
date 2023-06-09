using Microsoft.Extensions.Logging;
using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Services
{
    internal class MailEngine
    {
        private readonly ITemplateProcessor _templateProcessor;
        private readonly IMailSender _mailSender;
        private readonly IDictionary<string, string> _employeeEmails;
        private readonly ILogger _logger;
        private readonly Uri _host;

        private readonly string _timeheetTemplatePath =  @"TimesheetEmailTemplate.html";
        private readonly string _timeoffTemplatePath = @"TimeoffEmailTemplate.html";
        private const string _timesheetViewLink = "timesheets/{0}/employee/{1}/{2}";
        private const string _timeoffViewLink = "timeoffs/{0}/employee/{1}/{2}";

        internal MailEngine(ITemplateProcessor templateProcessor, IMailSender mailSender, IDictionary<string, string> employeeEmails, ILogger logger)
        {
            this._templateProcessor = templateProcessor;
            this._mailSender = mailSender;
            this._employeeEmails = employeeEmails;
            this._host = new Uri(templateProcessor.GetWebAppUri());

            this._logger = logger;
        }

        internal bool SendEmail<T>(T notificationItem)
            where T : BaseNotificationTemplate
        {
            if(notificationItem is TimeoffNotificationTemplate)
            {
                var linkFormat = new Uri(_host, _timeoffViewLink).ToString();
                notificationItem.Link = string.Format(linkFormat, notificationItem.ItemId, notificationItem.RelatedEmployeeId, ToTicks(notificationItem.ReferenceDate));
                return SendEmail(notificationItem, _timeoffTemplatePath);
            }
            else
            {
                var linkFormat = new Uri(_host, _timesheetViewLink).ToString();
                notificationItem.Link = string.Format(linkFormat, notificationItem.ItemId, notificationItem.RelatedEmployeeId, ToTicks(notificationItem.ReferenceDate));
                return SendEmail(notificationItem, _timeheetTemplatePath);
            }
        }

        private long? ToTicks(DateTime? referenceDate) => referenceDate != null ? new DateTimeOffset(referenceDate.Value).ToUnixTimeSeconds() : null;

        private bool SendEmail<T>(T notificationItem, string template)
            where T: BaseNotificationTemplate
        {
            _logger.LogInformation($"Setting template for {notificationItem.NotificationId}");

            _templateProcessor.SetTemplate(template);
            _employeeEmails.TryGetValue(notificationItem.EmployeeId, out var toEmail);

            if (!string.IsNullOrEmpty(toEmail))
            {
                var message = _templateProcessor.ProcessNotification(notificationItem);
                _mailSender.Send(message, toEmail, notificationItem.Subject);
                return true;
            }

            _logger.LogInformation($"Email not found for Employee {notificationItem.EmployeeId}");
            return false;
        }
    }
}

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Timesheet.EmailSender.Models;
using Timesheet.EmailSender.Repositories;

namespace Timesheet.EmailSender.Services
{
    internal class NotificationService : INotificationService
    {

        private MailEngine _mailEngine;
        private readonly ISettingRepository _settingsRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITemplateProcessor _templateProcessor;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ISettingRepository settingsRepository,
            INotificationRepository notificationRepository,
            IEmployeeRepository employeeRepository,
            ITemplateProcessor templateProcessor,
            ILogger<NotificationService> logger)
        {
            _settingsRepository = settingsRepository;
            _notificationRepository = notificationRepository;
            _employeeRepository = employeeRepository;
            _templateProcessor = templateProcessor;
            this._logger = logger;

            InitMailEngine();
        }

        private void InitMailEngine()
        {
            _logger.LogInformation($"{Environment.NewLine}Initializing Mail Engine");

            var settings = _settingsRepository.GetSMTPParameters();
            _logger.LogInformation($"SMTP parameters: {JsonConvert.SerializeObject(settings)}");

            var employeeEmails = _employeeRepository.GetEmails();

            IMailSender mailSender = new MailSender(settings, _logger);
            _mailEngine = new MailEngine(_templateProcessor, mailSender, employeeEmails, _logger);
        }

        public void SendNotifications()
        {
            IEnumerable<TimeoffNotificationTemplate> timeoffNotificationItems = _notificationRepository.GetTimeoffNotifications();
            IEnumerable<TimesheetNotificationTemplate> timesheetNotificationItems = _notificationRepository.GetTimesheetNotifications();

            _logger.LogInformation($"Total Timeoff Notifications to send : {timeoffNotificationItems.Count()} - {string.Join(',', timeoffNotificationItems.Select(n =>  n.EmployeeId).ToList())}");
            _logger.LogInformation($"Total Timesheet Notifications to send : {timesheetNotificationItems.Count()} - {string.Join(',', timesheetNotificationItems.Select(n => n.EmployeeId).ToList())}");

            SendNotifications(true, timeoffNotificationItems.ToArray());
            SendNotifications(true, timesheetNotificationItems.ToArray());
        }

        public void SendTestNotifications()
        {
            var timesheetNotificationItem = _notificationRepository.GetTestTimeoffNotification();
            if(timesheetNotificationItem is null)
            {
                throw new Exception("No available notification item for test");
            }

            timesheetNotificationItem.Subject = $"TEST - {timesheetNotificationItem.Subject}";
            SendNotifications(false, timesheetNotificationItem);
        }

        private void SendNotifications<T>(bool completeSend, params T[] notificationItems)
            where T : BaseNotificationTemplate
        {
            _logger.LogInformation("Sending notifications : ");
            foreach (var notificationItem in notificationItems)
            {
                _logger.LogInformation(JsonConvert.SerializeObject(notificationItem));

                _mailEngine.SendEmail(notificationItem);
                if (completeSend)
                {
                    _logger.LogInformation($"Notification {notificationItem.NotificationId} sent");
                    _notificationRepository.CompleteSend(notificationItem.NotificationId);
                    _logger.LogInformation($"Notification {notificationItem.NotificationId} processing done (mark as sent)");
                }
            }
            _logger.LogInformation("--DONE--");
        }
    }
}

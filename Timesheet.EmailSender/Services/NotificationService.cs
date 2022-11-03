using System.Configuration;
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

        public NotificationService(ISettingRepository settingsRepository,
            INotificationRepository notificationRepository,
            IEmployeeRepository employeeRepository,
            ITemplateProcessor templateProcessor)
        {
            _settingsRepository = settingsRepository;
            _notificationRepository = notificationRepository;
            _employeeRepository = employeeRepository;
            _templateProcessor = templateProcessor;

            InitMailEngine();

        }

        private void InitMailEngine()
        {
            var settings = _settingsRepository.GetSMTPParameters();
            var employeeEmails = _employeeRepository.GetEmails();

            IMailSender mailSender = new MailSender(settings);
            _mailEngine = new MailEngine(_templateProcessor, mailSender, employeeEmails);
        }

        public void SendNotifications()
        {
            IEnumerable<TimeoffNotificationTemplate> timeoffNotificationItems = _notificationRepository.GetTimeoffNotifications();
            IEnumerable<TimesheetNotificationTemplate> timesheetNotificationItems = _notificationRepository.GetTimesheetNotifications();

            SendNotifications(timeoffNotificationItems);
            SendNotifications(timesheetNotificationItems);
        }

        public void SendNotifications<T>(IEnumerable<T> notificationItems)
            where T : BaseNotificationTemplate
        {
            foreach (var notificationItem in notificationItems)
            {
                try
                {
                    _mailEngine.SendEmail(notificationItem);
                    notificationItem.Complete();
                }
                catch (Exception ex)
                {

                }
            }

            _notificationRepository.CompleteSend(notificationItems
                .Where(n => n.Sent)
                .Select(n => n.NotificationId)
                .ToList()
            );
        }
    }
}

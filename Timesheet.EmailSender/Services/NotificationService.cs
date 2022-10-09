using System.Configuration;
using Timesheet.EmailSender.Models;
using Timesheet.EmailSender.Repositories;

namespace Timesheet.EmailSender.Services
{
    internal class NotificationService : INotificationService
    {

        private MailEngine _mailEngine;
        private INotificationRepository _notificationRepository;

        public NotificationService(ISettingRepository settingsRepository, INotificationRepository notificationRepository)
        {
            InitMailEngine(settingsRepository);
            _notificationRepository = notificationRepository;
        }

        private void InitMailEngine(ISettingRepository settingsRepository)
        {
            var templatePath = ConfigurationManager.AppSettings["TemplatePath"];

            if(templatePath is null)
            {
                throw new Exception("Cannot find template path for mail notification");
            }

            var settings = settingsRepository.GetSMTPParameters();

            IMailSender mailSender = new MailSender(settings);
            var templateProcessor = new TemplateProcessor(templatePath);

            _mailEngine = new MailEngine(templateProcessor, mailSender);
        }

        public void SendNotifications()
        {
            IEnumerable<NotificationItem> notificationItems = _notificationRepository.Get();
            foreach (var notificationItem in notificationItems)
            {
                try
                {
                    _mailEngine.SendEmail(notificationItem);
                    notificationItem.SetCompleted();
                    _notificationRepository.CompleteTransaction();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}

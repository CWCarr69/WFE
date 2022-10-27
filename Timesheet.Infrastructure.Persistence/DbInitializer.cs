using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Models.Settings;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Initialize(TimesheetDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Settings.Any())
            {
                Setting[] settings = GetSettings();

                foreach (var setting in settings)
                {
                    context.Settings.Add(setting);
                }
            }

            if (!context.Notifications.Any())
            {
                Notification[] notifications = GetNotifications();

                foreach (var notification in notifications)
                {
                    context.Notifications.Add(notification);
                }
            }

            context.SaveChanges();
        }

        private static Setting[] GetSettings()
        {
            return new Setting[]
            {
                Setting.Create("AUTHENTICATION_SERVER", "10.73.4.10", "Active directory server"),

                Setting.Create("SMTP_USE_SSL", "0", "Use ssl for smtp connection (true, false)"),
                Setting.Create("SMTP_SET_EMAIL", "0", "True/False for SMTP email setting"),
                Setting.Create("SMTP_ACCOUNT_PASSWORD", "", "Password of email account"),
                Setting.Create("SMTP_PORT", "25", "Smtp email server port"),
                Setting.Create("SMTP_ACCOUNT_ADDRESS", "25", "Smtp email server address"),
                Setting.Create("SMTP_ACCOUNT_USERNAME", "", "User name of email account"),

                Setting.Create("NOTIFICATION_HR", "HR Support Email", "HR Support Email"),
                Setting.Create("NOTIFICATION_WFE", "HR@WilsonFire.com", "Wilson Fire Equip. and Services"),

                Setting.Create("FDP_USERNAME", "FPIntegration@wilsonfire.com", "UserName"),
                Setting.Create("FDP_PASSWORD", "0iRP0qilgyYM7SCipFmp", "Password"),
                Setting.Create("FDP_URL", "https://wilsonfiredev.fieldpointonline.com/Services/FPDTSWS.asmx", "Service Url"),
                Setting.Create("FDP_Domain", "WilsonFire", "Service Domain"),

                Setting.Create("VALIDATION_NOTES_LENGTH", "1024", "Notes Length (bytes)"),
            };
        }

        private static Notification[] GetNotifications()
        {
            return new Notification[]
            {
                Notification.Create(0, NotificationType.TIMESHEET, TimesheetEntryStatus.IN_PROGRESS.ToString()),
                Notification.Create(0, NotificationType.TIMESHEET, TimesheetEntryStatus.APPROVED.ToString()),
                Notification.Create(0, NotificationType.TIMESHEET, TimesheetEntryStatus.REJECTED.ToString()),
                Notification.Create(0, NotificationType.TIMESHEET, TimesheetEntryStatus.SUBMITTED.ToString()),
                Notification.Create(0, NotificationType.TIMESHEET, TimesheetStatus.FINALIZED.ToString()),
                
                Notification.Create(0, NotificationType.TIMEOFF, TimeoffStatus.IN_PROGRESS.ToString()),
                Notification.Create(0, NotificationType.TIMEOFF, TimeoffStatus.SUBMITTED.ToString()),
                Notification.Create(0, NotificationType.TIMEOFF, TimeoffStatus.APPROVED.ToString()),
                Notification.Create(0, NotificationType.TIMEOFF, TimeoffStatus.REJECTED.ToString())
            };
        }
    }
}

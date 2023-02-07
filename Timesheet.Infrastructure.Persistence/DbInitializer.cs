using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Models.Settings;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Models.Referential;

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

            if (!context.PayrollTypes.Any())
            {
                PayrollTypes[] payrollTypes = GetPayrollTypes();

                foreach (var payrollType in payrollTypes)
                {
                    context.PayrollTypes.Add(payrollType);
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
                Setting.Create("FDP_RetainFiles", "FALSE", "Retain FP file uploads"),
                Setting.Create("FDP_UploadFrequency", "1", "Upload frequency in hours"),
                
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

        private static PayrollTypes[] GetPayrollTypes()
        {
            return new PayrollTypes[]
            {
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.REGULAR, "REGULAR", PayrollTypesCategory.ALL, "REG", true),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.OVERTIME, "OVERTIME", PayrollTypesCategory.ALL, "OT", true),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.HOLIDAY, "HOLIDAY", PayrollTypesCategory.TIMEOFF, "", true),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.PERSONAL, "PERSONAL", PayrollTypesCategory.TIMEOFF, "VAC", true),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.VACATION, "VACATION", PayrollTypesCategory.TIMEOFF, "VAC", true),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.UNPAID, "UNPAID", PayrollTypesCategory.TIMEOFF, "UNPAID", false),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.JURY_DUTY, "JURY_DUTY", PayrollTypesCategory.TIMEOFF, "EJURY", true),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.BERV, "BERV", PayrollTypesCategory.TIMEOFF, "BERV", true),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.SHOP, "SHOP", PayrollTypesCategory.TIMEOFF, "SHOP", false),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.TRAINING, "TRAINING", PayrollTypesCategory.TIMEOFF, "TRAINING", false),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.OTHERS_WITHOUT_APPROVAL, "OTHERS_WITHOUT_APPROVAL", PayrollTypesCategory.TIMEOFF, "OTHERS_WITHOUT_APPROVAL", false),
                PayrollTypes.Create((int) TimesheetFixedPayrollCodeEnum.OTHERS_WITH_APPROVAL, "OTHERS_WITH_APPROVAL", PayrollTypesCategory.TIMEOFF, "OTHERS_WITH_APPROVAL", true),
            };
        }
    }
}

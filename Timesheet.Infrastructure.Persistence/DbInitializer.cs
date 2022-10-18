using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Domain.Models.Settings;

namespace Timesheet.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Initialize(TimesheetDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Settings.Any())
            {
                return;
            }

            Setting[] settings = GetSettings();

            foreach (var setting in settings)
            {
                context.Add(setting);
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

                Setting.Create("FDP_USERNAME", "", "UserName"),
                Setting.Create("FDP_PASSWORD", "", "Password"),
                Setting.Create("FDP_URL", "", "Service Url"),
                Setting.Create("FDP_Domain", "WilsonFire", "Service Domain"),

                Setting.Create("VALIDATION_NOTES_LENGTH", "1024", "Notes Length (bytes)"),
            };
        }
    }
}

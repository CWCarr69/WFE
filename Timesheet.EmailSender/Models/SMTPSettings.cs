namespace Timesheet.EmailSender.Models
{
    public class SMTPSettings
    {
        public bool SMTP_UseSSL { get;private set; } = false;
        public int SMTP_Port { get;private set; } = 25;
        public string SMTP_Server { get;private set; }
        public string SMTP_Username { get;private set; }
        public string SMTP_Password { get;private set; }
        public string SMTP_From { get;private set; }
        public string SMTP_FromDisplayName { get;private set; }

        public static SMTPSettings CreateFromConfigurationList(IEnumerable<(string Name, string Value)> configurations)
        {
            var SmtpSettings = new SMTPSettings();
            foreach(var setting in configurations)
            {
                if (nameof(SMTP_UseSSL).ToLower() == setting.Name.ToLower())
                {
                    SmtpSettings.SMTP_UseSSL = int.Parse(setting.Value) == 1;
                }

                if (nameof(SMTP_Port).ToLower() == setting.Name.ToLower())
                {
                    SmtpSettings.SMTP_Port = int.Parse(setting.Value);
                }

                if (nameof(SMTP_Server).ToLower() == setting.Name.ToLower())
                {
                    SmtpSettings.SMTP_Server = setting.Value;
                }

                if (nameof(SMTP_Username).ToLower() == setting.Name.ToLower())
                {
                    SmtpSettings.SMTP_Username = setting.Value;
                }

                if (nameof(SMTP_Password).ToLower() == setting.Name.ToLower())
                {
                    SmtpSettings.SMTP_Password = setting.Value;
                }

                if ("NOTIFICATION_HRE_DISPLAY_NAME".ToLower() == setting.Name.ToLower())
                {
                    SmtpSettings.SMTP_FromDisplayName = setting.Value;
                }

                if ("NOTIFICATION_HRE_EMAIL".ToLower() == setting.Name.ToLower())
                {
                    SmtpSettings.SMTP_From = setting.Value;
                }
            }

            if (string.IsNullOrEmpty(SmtpSettings.SMTP_Server) 
                || string.IsNullOrEmpty(SmtpSettings.SMTP_Username)
                || string.IsNullOrEmpty(SmtpSettings.SMTP_Password)
                || string.IsNullOrEmpty(SmtpSettings.SMTP_From))
            {
                throw new Exception("Smtp Configurations are not available");
            }

            return SmtpSettings;
        }
    }
}
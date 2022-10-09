namespace Timesheet.EmailSender.Models
{
    public class SMTPSettings
    {
        public bool SMTP_UseSSL { get;private set; } = false;
        public int SMTP_Port { get;private set; } = 25;
        public string SMTP_EmailServer { get;private set; }
        public string SMTP_Username { get;private set; }
        public string SMTP_Password { get;private set; }
        public string SMTP_Email { get;private set; } 
        public string SMTP_SenderName { get;private set; }

        public static SMTPSettings CreateFromConfigurationList(IEnumerable<(string Name, string Value)> configurations)
        {
            var SmtpSettings = new SMTPSettings();
            foreach(var setting in configurations)
            {
                if (nameof(SMTP_UseSSL) == setting.Name)
                {
                    SmtpSettings.SMTP_UseSSL = bool.Parse(setting.Value);
                }

                if (nameof(SMTP_Port) == setting.Name)
                {
                    SmtpSettings.SMTP_Port = int.Parse(setting.Value);
                }

                if (nameof(SMTP_EmailServer) == setting.Name)
                {
                    SmtpSettings.SMTP_EmailServer = setting.Value;
                }

                if (nameof(SMTP_Username) == setting.Name)
                {
                    SmtpSettings.SMTP_Username = setting.Value;
                }

                if (nameof(SMTP_Password) == setting.Name)
                {
                    SmtpSettings.SMTP_Password = setting.Value;
                }

                if (nameof(SMTP_Email) == setting.Name)
                {
                    SmtpSettings.SMTP_Email = setting.Value;
                }

                if (nameof(SMTP_SenderName) == setting.Name)
                {
                    SmtpSettings.SMTP_SenderName = setting.Value;
                }
            }

            if (string.IsNullOrEmpty(SmtpSettings.SMTP_EmailServer) 
                || string.IsNullOrEmpty(SmtpSettings.SMTP_Username)
                || string.IsNullOrEmpty(SmtpSettings.SMTP_Password)
                || string.IsNullOrEmpty(SmtpSettings.SMTP_Email))
            {
                throw new Exception("Smtp Configurations are not available");
            }

            return SmtpSettings;
        }
    }
}
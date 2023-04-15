using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Services
{
    internal class MailSender : IMailSender
    {
        private SMTPSettings _settings;

        public MailSender(SMTPSettings settings)
        {
            this._settings = settings;
        }

        public void Send(string message, string to, string subject)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();

                // Use custom certificate validation:
                Disable_CertificateValidation();

                mail.From = new MailAddress(_settings.SMTP_Username, _settings.SMTP_FromDisplayName);
                mail.To.Add(new MailAddress(to));
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = message;
                client.Port = _settings.SMTP_Port;
                client.Host = _settings.SMTP_Server; //for gmail host  
                client.EnableSsl = _settings.SMTP_UseSSL;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_settings.SMTP_Username, _settings.SMTP_Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.Send(mail);
            }
            catch (Exception) { }
        }

        //TODO : Change this to use appropriate valdiation", true
        private void Disable_CertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                ) {
                    return true;
                };
        }
    }
}
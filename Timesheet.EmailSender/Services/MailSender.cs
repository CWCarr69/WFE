using System.Net;
using System.Net.Mail;
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
                SmtpClient smtp = new SmtpClient();

                mail.From = new MailAddress(_settings.SMTP_Email);
                mail.To.Add(new MailAddress(to));
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = message;
                smtp.Port = 587;
                smtp.Host = _settings.SMTP_EmailServer; //for gmail host  
                smtp.EnableSsl = _settings.SMTP_UseSSL;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_settings.SMTP_Username, _settings.SMTP_Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);
            }
            catch (Exception) { }
        }
    }
}
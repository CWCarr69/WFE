namespace Timesheet.EmailSender.Services
{
    internal interface IMailSender
    {
        void Send(string message, string to, string subject);
    }
}
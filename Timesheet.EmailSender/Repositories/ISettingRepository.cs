using Timesheet.EmailSender.Models;

namespace Timesheet.EmailSender.Repositories
{
    internal interface ISettingRepository
    {
        SMTPSettings GetSMTPParameters();
    }
}
namespace Timesheet.EmailSender.Repositories
{
    internal interface IEmployeeRepository
    {
        IDictionary<string, string> GetEmails();
    }
}
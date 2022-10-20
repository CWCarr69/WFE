using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.Exceptions
{
    public sealed class CannotExecuteCommandException : DomainException
    {
        public CannotExecuteCommandException(string command, string employeeId) 
        : base($"Timesheet.Application", 403, $"Command {command} cannot be execute by {employeeId}.")
        {
        }
    }
}

using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.Exceptions
{
    public sealed class CannotExecuteCommandWithoutAuthorException : DomainException
    {
        public CannotExecuteCommandWithoutAuthorException(string command) 
        : base($"Timesheet.Application", 403, $"Author must be provided in order to execute Command {command}.")
        {
        }
    }
}

namespace Timesheet.Domain.Exceptions
{
    internal class TimesheetEntryIsNotDeletableException : DomainException
    {
        public TimesheetEntryIsNotDeletableException(string id) :
            base("TimesheetEntry.Delete.Denied", 400, $"TimesheetEntry ({id}) cannot be deleted")
        {
        }
    }
}

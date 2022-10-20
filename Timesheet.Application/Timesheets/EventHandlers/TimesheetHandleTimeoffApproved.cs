using Timesheet.DomainEvents.Employees;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal class TimesheetHandleTimeoffApproved : IEventHandler<TimeoffApproved>
    {
        public Task Handle(TimeoffApproved @event)
        {
            throw new NotImplementedException();
        }
    }
}

using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal class TimeoffApprovedTimesheet : IEventHandler<HolidayAdded>
    {
        public Task Handle(HolidayAdded @event)
        {
            throw new NotImplementedException();
        }
    }
}

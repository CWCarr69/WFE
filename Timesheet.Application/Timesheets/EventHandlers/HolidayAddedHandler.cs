using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal sealed class HolidayAddedHandler : IEventHandler<HolidayAdded>
    {
        public void Handle(HolidayAdded @event)
        {
            throw new NotImplementedException();
        }
    }
}

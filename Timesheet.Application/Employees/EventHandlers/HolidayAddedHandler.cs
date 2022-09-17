using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Employees.EventHandlers
{
    internal sealed class HolidayAddedHandler : IEventHandler<HolidayAdded>
    {
        public void Handle(HolidayAdded @event)
        {
            throw new NotImplementedException();
        }
    }
}

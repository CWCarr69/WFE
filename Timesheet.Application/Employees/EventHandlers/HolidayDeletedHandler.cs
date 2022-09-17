using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Employees.EventHandlers
{
    internal sealed class HolidayDeletedHandler : IEventHandler<HolidayDeleted>
    {
        public void Handle(HolidayDeleted @event)
        {
            throw new NotImplementedException();
        }
    }
}

using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal sealed class HolidayDeletedHandler : IEventHandler<HolidayDeleted>
    {
        public Task Handle(HolidayDeleted @event)
        {
            throw new NotImplementedException();
        }
    }
}

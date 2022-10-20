using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal sealed class TimesheetHandleHolidayAdded : IEventHandler<HolidayAdded>
    {
        public Task Handle(HolidayAdded @event)
        {
            throw new NotImplementedException();
        }
    }
}

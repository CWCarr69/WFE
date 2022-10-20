using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal sealed class TimehsheetHandleHolidayGeneralInformationsUpdated : IEventHandler<HolidayGeneralInformationsUpdated>
    {
        public Task Handle(HolidayGeneralInformationsUpdated @event)
        {
            throw new NotImplementedException();
        }
    }
}

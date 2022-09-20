using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal sealed class HolidayGeneralInformationsUpdatedHandler : IEventHandler<HolidayGeneralInformationsUpdated>
    {
        public void Handle(HolidayGeneralInformationsUpdated @event)
        {
            throw new NotImplementedException();
        }
    }
}

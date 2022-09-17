using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Employees.EventHandlers
{
    internal sealed class HolidayGeneralInformationsUpdatedHandler : IEventHandler<HolidayGeneralInformationsUpdated>
    {
        public void Handle(HolidayGeneralInformationsUpdated @event)
        {
            throw new NotImplementedException();
        }
    }
}

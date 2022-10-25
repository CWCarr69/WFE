using Timesheet.Domain.DomainEvents;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers.Holidays
{
    internal sealed class TimehsheetHandleHolidayGeneralInformationsUpdated : IEventHandler<HolidayGeneralInformationsUpdated>
    {
        private readonly ITimesheetReadRepository _readRepository;

        public TimehsheetHandleHolidayGeneralInformationsUpdated(
            ITimesheetReadRepository readRepository)
        {
            this._readRepository = readRepository;
        }

        public async Task Handle(HolidayGeneralInformationsUpdated @event)
        {
            var timesheets = await _readRepository.GetTimesheetByHoliday(@event.Id);
            foreach (var timesheet in timesheets)
            {
                timesheet.UpdateHoliday(@event.Id, @event.Description);
            }
        }
    }
}

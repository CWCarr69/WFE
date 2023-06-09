using Timesheet.Application.Shared;
using Timesheet.Domain.DomainEvents.Holidays;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers.Holidays
{
    internal sealed class TimehsheetHandleHolidayGeneralInformationsUpdated : BaseEventHandler<HolidayGeneralInformationsUpdated>
    {
        private readonly ITimesheetReadRepository _readRepository;

        public TimehsheetHandleHolidayGeneralInformationsUpdated(
            ITimesheetReadRepository readRepository,
            IUnitOfWork unitOfWork): base(unitOfWork)
        {
            this._readRepository = readRepository;
        }

        public override async Task HandleEvent(HolidayGeneralInformationsUpdated @event)
        {
            var timesheets = await _readRepository.GetTimesheetByHoliday(@event.Id);
            foreach (var timesheet in timesheets)
            {
                timesheet.UpdateHoliday(@event.Id, @event.Description);
            }
        }
    }
}

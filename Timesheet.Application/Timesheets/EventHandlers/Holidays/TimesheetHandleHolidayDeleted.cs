using Timesheet.Domain.DomainEvents;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers.Holidays
{
    internal sealed class TimesheetHandleHolidayDeleted : IEventHandler<HolidayDeleted>
    {

        private readonly ITimesheetReadRepository _readRepository;

        public TimesheetHandleHolidayDeleted(ITimesheetReadRepository readRepository)
        {
            this._readRepository = readRepository;
        }

        public async Task Handle(HolidayDeleted @event)
        {
            var timesheets = await _readRepository.GetTimesheetByHoliday(@event.Id);
            foreach (var timesheet in timesheets)
            {
                timesheet.DeleteHoliday(@event.Id);
            }
        }
    }
}

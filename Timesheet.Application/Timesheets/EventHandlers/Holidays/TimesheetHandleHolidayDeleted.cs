using Timesheet.Application.Shared;
using Timesheet.Domain.DomainEvents.Holidays;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers.Holidays
{
    internal sealed class TimesheetHandleHolidayDeleted : BaseEventHandler<HolidayDeleted>
    {

        private readonly ITimesheetReadRepository _readRepository;

        public TimesheetHandleHolidayDeleted(ITimesheetReadRepository readRepository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this._readRepository = readRepository;
        }

        public override async Task HandleEvent(HolidayDeleted @event)
        {
            var timesheets = await _readRepository.GetTimesheetByHoliday(@event.Id);
            foreach (var timesheet in timesheets)
            {
                timesheet.DeleteHoliday(@event.Id);
            }
        }
    }
}

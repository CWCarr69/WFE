using Timesheet.Domain.DomainEvents;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers.Holidays
{
    internal sealed class TimesheetHandleHolidayAdded : IEventHandler<HolidayAdded>
    {
        private readonly ITimesheetReadRepository _readRepository;
        private readonly IWriteRepository<TimesheetHeader> _writeRepository;

        public TimesheetHandleHolidayAdded(
            ITimesheetReadRepository readRepository,
            IWriteRepository<TimesheetHeader> writeRepository)
        {
            this._readRepository = readRepository;
            this._writeRepository = writeRepository;
        }

        public async Task Handle(HolidayAdded @event)
        {
            var timesheetHoliday = new TimesheetHoliday(@event.Id, @event.Date, @event.Description);

            var timesheets = await _readRepository.GetTimesheetByDate(@event.Date);

            if (timesheets == null || !timesheets.Any(t => t.Type == TimesheetType.WEEKLY))
            {
                var timesheetWeekly = TimesheetHeader.CreateWeeklyTimesheet(@event.Date);
                await _writeRepository.Add(timesheetWeekly);
            }

            if (timesheets == null || !timesheets.Any(t => t.Type == TimesheetType.WEEKLY))
            {
                var timesheetMonthly = TimesheetHeader.CreateMonthlyTimesheet(@event.Date);
                await _writeRepository.Add(timesheetMonthly);
            }

            foreach(var timesheet in timesheets)
            {
                timesheet.AddHoliday(timesheetHoliday);
            }
        }
    }
}

using Timesheet.Application.Shared;
using Timesheet.Domain.DomainEvents;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers.Holidays
{
    internal sealed class TimesheetHandleHolidayAdded : BaseEventHandler<HolidayAdded>
    {
        private readonly ITimesheetReadRepository _readRepository;
        private readonly IWriteRepository<TimesheetHeader> _writeRepository;

        public TimesheetHandleHolidayAdded(
            ITimesheetReadRepository readRepository,
            IWriteRepository<TimesheetHeader> writeRepository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this._readRepository = readRepository;
            this._writeRepository = writeRepository;
        }

        public override async Task HandleEvent(HolidayAdded @event)
        {
            var timesheetHoliday = new TimesheetHoliday(@event.Id, @event.Date, @event.Description);

            var timesheets = (await _readRepository.GetTimesheetByDate(@event.Date))
                ?.ToList() ?? new List<TimesheetHeader>();

            if (!timesheets.Any(t => t.Type == TimesheetType.WEEKLY))
            {
                var timesheetWeekly = TimesheetHeader.CreateWeeklyTimesheet(@event.Date);
                await _writeRepository.Add(timesheetWeekly);
                timesheets.Add(timesheetWeekly);
            }

            if (!timesheets.Any(t => t.Type == TimesheetType.SALARLY))
            {
                var timesheetMonthly = TimesheetHeader.CreateMonthlyTimesheet(@event.Date);
                await _writeRepository.Add(timesheetMonthly);
                timesheets.Add(timesheetMonthly);
            }

            foreach (var timesheet in timesheets)
            {
                timesheet.AddHoliday(timesheetHoliday);
            }
        }
    }
}

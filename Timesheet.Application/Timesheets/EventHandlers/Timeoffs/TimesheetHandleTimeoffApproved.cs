using Timesheet.Application.Shared;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;
using Timesheet.DomainEvents.Employees;
using Timesheet.Models.Referential;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal class TimesheetHandleTimeoffApproved : BaseEventHandler<TimeoffApproved>
    {
        private readonly ITimesheetReadRepository _readRepository;
        private readonly IWriteRepository<TimesheetHeader> _writeRepository;
        private readonly IHolidayReadRepository _holidayRepository;

        public TimesheetHandleTimeoffApproved(
            ITimesheetReadRepository readRepository,
            IWriteRepository<TimesheetHeader> writeRepository,
            IHolidayReadRepository holidayRepository,
            IUnitOfWork unitOfWork): base(unitOfWork)
        {
            this._readRepository = readRepository;
            this._writeRepository = writeRepository;
            _holidayRepository = holidayRepository;
        }

        public override async Task HandleEvent(TimeoffApproved @event)
        {
            //var @type = EventTypeToTimesheetPayrollCode(@event.Type);
            foreach (var entry in @event.TimeoffEntries)
            {
                var entryExists = await _readRepository.DoesEntryExists(entry.Id);

                if (entryExists)
                {
                    return;
                }

                if(entry.TypeId == (int)TimesheetFixedPayrollCodeEnum.HOLIDAY)
                {
                    CheckHolidayDoesntExist(entry);
                }

                var timesheetEntry = TimesheetEntry.CreateFromApprovedTimeOff(
                    entry.Id,
                    entry.EmployeeId,
                    entry.RequestDate,
                    entry.TypeId,
                    entry.Hours,
                    entry.Description,
                    TimesheetEntryStatus.APPROVED);

                var timesheet = entry.IsSalaried
                        ? TimesheetHeader.CreateMonthlyTimesheet(entry.RequestDate)
                        : TimesheetHeader.CreateWeeklyTimesheet(entry.RequestDate);

                var alreadyAddedtimesheet = await _readRepository.GetTimesheet(timesheet.Id);

                if (alreadyAddedtimesheet is null)
                {
                    await _writeRepository.Add(timesheet);
                    alreadyAddedtimesheet = timesheet;
                }

                alreadyAddedtimesheet.AddTimesheetEntry(timesheetEntry);
            }
        }

        private void CheckHolidayDoesntExist(TimeoffApprovedEntry entry)
        {
            if (_holidayRepository.GetByDate(entry.RequestDate) is not null)
            {
                throw new HolidayAlreadyExistException(entry.RequestDate);
            }
        }
    }
}

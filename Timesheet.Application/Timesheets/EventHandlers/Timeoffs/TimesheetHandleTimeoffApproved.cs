using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;
using Timesheet.DomainEvents.Employees;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal class TimesheetHandleTimeoffApproved : IEventHandler<TimeoffApproved>
    {
        private readonly ITimesheetReadRepository _readRepository;
        private readonly IWriteRepository<TimesheetHeader> _writeRepository;


        public TimesheetHandleTimeoffApproved(
            ITimesheetReadRepository readRepository,
            IWriteRepository<TimesheetHeader> writeRepository)
        {
            this._readRepository = readRepository;
            this._writeRepository = writeRepository;
        }

        public async Task Handle(TimeoffApproved @event)
        {
            //var @type = EventTypeToTimesheetPayrollCode(@event.Type);
            foreach (var entry in @event.TimeoffEntries)
            {
                var entryExists = await _readRepository.DoesEntryExists(entry.Id);

                if (entryExists)
                {
                    return;
                }

                var timesheetEntry = new TimesheetEntry(
                    entry.Id,
                    entry.EmployeeId,
                    entry.RequestDate,
                    entry.TypeId,
                    entry.Hours,
                    entry.Description,
                    TimesheetEntryStatus.APPROVED);

                var timesheet = entry.IsSalaried
                        ? TimesheetHeader.CreateWeeklyTimesheet(entry.RequestDate)
                        : TimesheetHeader.CreateMonthlyTimesheet(entry.RequestDate);

                var alreadyAddedtimesheet = await _readRepository.GetTimesheet(timesheet.Id);

                if (alreadyAddedtimesheet is null)
                {
                    await _writeRepository.Add(timesheet);
                    alreadyAddedtimesheet = timesheet;
                }

                alreadyAddedtimesheet.AddTimesheetEntry(timesheetEntry);
            }
        }

        //private TimesheetPayrollCode EventTypeToTimesheetPayrollCode(string @type)
        //{
        //    return new TimesheetPayrollCode(@type.Code);
        //    //if (@type == TimeoffType.BERV.ToString()) return TimesheetPayrollCode.BERV.ToString();
        //    //if (@type == TimeoffType.UNPAID.ToString()) return TimesheetPayrollCode.UNPAID.ToString();
        //    //if (@type == TimeoffType.VACATION.ToString()) return TimesheetPayrollCode.VACATION.ToString();
        //    //if (@type == TimeoffType.SHOP.ToString()) return TimesheetPayrollCode.SHOP.ToString();
        //    //if (@type == TimeoffType.JURY_DUTY.ToString()) return TimesheetPayrollCode.JURY_DUTY.ToString();
        //    //if (@type == TimeoffType.PERSONAL.ToString()) return TimesheetPayrollCode.PERSONAL.ToString();

        //    //return TimesheetPayrollCode.REGULAR.ToString();
        //}
    }
}

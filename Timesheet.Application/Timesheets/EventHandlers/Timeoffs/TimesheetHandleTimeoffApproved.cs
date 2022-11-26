using Timesheet.Domain.Models.Employees;
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
            var @type = EventTypeToTimesheetPayrollCode(@event.Type);

            var timesheetEntry = new TimesheetEntry(
                @event.Id,
                @event.EmployeeId,
                @event.RequestDate,
                @type,
                @event.Hours,
                @event.Description);

            var timesheet = @event.IsSalaried
                    ? TimesheetHeader.CreateWeeklyTimesheet(@event.RequestDate)
                    : TimesheetHeader.CreateMonthlyTimesheet(@event.RequestDate);

            var alreadyAddedtimesheet = await _readRepository.GetTimesheet(timesheet.Id);

            if(alreadyAddedtimesheet is null)
            {
                await _writeRepository.Add(timesheet);
            }

            timesheet.AddTimesheetEntry(timesheetEntry);
        }

        private string EventTypeToTimesheetPayrollCode(string @type)
        {
            if(@type == TimeoffType.BERV.ToString()) return TimesheetPayrollCode.BERV.ToString();
            if(@type == TimeoffType.UNPAID.ToString()) return TimesheetPayrollCode.UNPAID.ToString();
            if(@type == TimeoffType.VACATION.ToString()) return TimesheetPayrollCode.VACATION.ToString();
            if(@type == TimeoffType.SHOP.ToString()) return TimesheetPayrollCode.SHOP.ToString();
            if(@type == TimeoffType.JURY_DUTY.ToString()) return TimesheetPayrollCode.JURY_DUTY.ToString();
            if(@type == TimeoffType.PERSONAL.ToString()) return TimesheetPayrollCode.PERSONAL.ToString();

            return TimesheetPayrollCode.REGULAR.ToString();
        }
    }
}

using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;
using Timesheet.DomainEvents.Employees;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal class TimesheetHandleTimeoffApproved : IEventHandler<TimeoffApproved>
    {
        private readonly ITimesheetReadRepository _readRepository;
        private readonly IWriteRepository<TimesheetHeader> writeRepository;

        public TimesheetHandleTimeoffApproved(
            ITimesheetReadRepository readRepository,
            IWriteRepository<TimesheetHeader> writeRepository)
        {
            this._readRepository = readRepository;
            this.writeRepository = writeRepository;
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

            var timesheetType = @event.IsSalaried ? TimesheetType.SALARLY : TimesheetType.WEEKLY; 
            var timesheet = await _readRepository.GetTimesheetByDate(@event.RequestDate, timesheetType);

            if(timesheet is null)
            {
                timesheet = @event.IsSalaried
                    ? TimesheetHeader.CreateWeeklyTimesheet(@event.RequestDate)
                    : TimesheetHeader.CreateMonthlyTimesheet(@event.RequestDate);

                await writeRepository.Add(timesheet);
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

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
            var timesheetEntry = new TimesheetEntry(
                @event.Id,
                @event.EmployeeId,
                @event.RequestDate,
                @event.Type.ToString(),
                @event.Hours,
                @event.Description);

            var timesheetType = @event.IsSalaried ? TimesheetType.SALARLY : TimesheetType.WEEKLY; 
            var timesheet = await _readRepository.GetTimesheetByDate(@event.RequestDate, timesheetType);

            if(timesheet != null)
            {
                timesheet = @event.IsSalaried
                    ? TimesheetHeader.CreateWeeklyTimesheet(@event.RequestDate)
                    : TimesheetHeader.CreateMonthlyTimesheet(@event.RequestDate);

                await writeRepository.Add(timesheet);
            }

            timesheet.AddTimesheetEntry(timesheetEntry);
        }
    }
}

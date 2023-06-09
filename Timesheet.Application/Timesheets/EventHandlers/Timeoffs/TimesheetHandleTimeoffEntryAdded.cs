using Timesheet.Application.Shared;
using Timesheet.Domain.DomainEvents.Employees;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal class TimesheetHandleTimeoffEntryAdded : BaseEventHandler<TimeoffEntryAdded>
    {
        private readonly ITimesheetReadRepository _readRepository;

        public TimesheetHandleTimeoffEntryAdded(ITimesheetReadRepository readRepository, IUnitOfWork unitOfWork): base(unitOfWork)
        {
            this._readRepository = readRepository;
        }

        public override async Task HandleEvent(TimeoffEntryAdded @event)
        {
            var timesheetType = @event.isSalaried ? TimesheetType.SALARLY : TimesheetType.WEEKLY;
            var timesheet = await _readRepository.GetTimesheetByDate(@event.RequestDate, timesheetType);

            var timesheetIsFinalized = timesheet != null && timesheet.Status == TimesheetStatus.FINALIZED;

            if(timesheetIsFinalized)
            {
                throw new TimesheetAlreadyFinalized(
                    $"Cannot add entry ({@event.RequestDate.ToShortDateString()}) to time off.",
                    timesheet.PayrollPeriod,
                    timesheet.StartDate.ToShortDateString(),
                    timesheet.EndDate.ToShortDateString()
                );
            }
        }
    }
}

using Timesheet.Application.Shared;
using Timesheet.Domain.DomainEvents.Timesheets;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal class EmployeeHandleTimesheetFinalized: BaseEventHandler<TimesheetFinalized>
    {
        private readonly IEmployeeReadRepository _readRepository;

        public EmployeeHandleTimesheetFinalized(IEmployeeReadRepository readRepository, 
            IUnitOfWork unitOfWork): base(unitOfWork)
        {
            this._readRepository = readRepository;
        }

        public override async Task HandleEvent(TimesheetFinalized @event)
        {
            var employees = await _readRepository.GetEmployeeWithTimeoffsInPeriod(@event.startDate, @event.endDate);
            employees.ToList().ForEach(employee =>
            {
                employee.RejectEntries(@event.startDate, @event.endDate);
            });
        }
    }
}

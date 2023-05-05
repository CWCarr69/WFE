using Timesheet.Application.Shared;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;
using Timesheet.DomainEvents.Timesheets;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal class EmployeeHandleTimesheetRejected : BaseEventHandler<TimesheetRejected>
    {
        private readonly IEmployeeReadRepository _readRepository;

        public EmployeeHandleTimesheetRejected(IEmployeeReadRepository readRepository, 
            IUnitOfWork unitOfWork): base(unitOfWork)
        {
            this._readRepository = readRepository;
        }

        public override async Task HandleEvent(TimesheetRejected @event)
        {
            var employee = await _readRepository.GetEmployee(@event.EmployeeId);

            if(employee is null) 
            {
                throw new EntityNotFoundException<Employee>(@event.EmployeeId);
            }

            @event.Dates.ForEach(date => employee.RejectEntries(date));
        }
    }
}

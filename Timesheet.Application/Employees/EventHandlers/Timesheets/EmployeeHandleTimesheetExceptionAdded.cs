using Timesheet.Application.Shared;
using Timesheet.Domain.DomainEvents.Timesheets;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers
{
    internal class EmployeeHandleTimesheetExceptionAdded : BaseEventHandler<TimesheetExceptionAdded>
    {
        private readonly IEmployeeReadRepository _readRepository;

        public EmployeeHandleTimesheetExceptionAdded(IEmployeeReadRepository readRepository, 
            IUnitOfWork unitOfWork): base(unitOfWork)
        {
            this._readRepository = readRepository;
        }

        public override async Task HandleEvent(TimesheetExceptionAdded @event)
        {
            var employee = await _readRepository.GetEmployee(@event.EmployeeId);

            if(employee is null) 
            {
                throw new EntityNotFoundException<Employee>(@event.EmployeeId);
            }

            employee.DeleteEntry(@event.EntryId);
        }
    }
}

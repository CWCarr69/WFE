using Timesheet.Application.Employees.Commands;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class CreateTimeoffCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, CreateTimeoff>
    {
        private readonly IDispatcher _dispatcher;

        public CreateTimeoffCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork
            ) : base(auditHandler, readRepository, dispatcher, unitOfWork)
        {
            _dispatcher = dispatcher;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(CreateTimeoff command, CancellationToken token)
        {
            if (command.RequestEndDate > command.RequestStartDate)
            {
                throw new TimeoffInvalidDateIntervalException(command.RequestStartDate, command.RequestEndDate);
            }

            Employee employee = await GetEmployee(command.EmployeeId);

            var timeoff = employee.CreateTimeoff(command.RequestStartDate, command.RequestEndDate, command.EmployeeComment, command.ApproverComment);
            this.RelatedAuditableEntity = timeoff;

            var commandContext = new Dictionary<string, object>() { 
                { "Employee", employee },
                { "Timeoff", timeoff }
            };
            command.Entries?.ToList().ForEach(async entryCommand => await _dispatcher.RunSubCommand(entryCommand, commandContext, token));

            return employee.GetDomainEvents();
        }
    }
}

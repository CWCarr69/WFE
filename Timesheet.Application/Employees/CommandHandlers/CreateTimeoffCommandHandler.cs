using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
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
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations
            ) : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            _dispatcher = dispatcher;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(CreateTimeoff command, CancellationToken token)
        {
            if (command.RequestEndDate < command.RequestStartDate)
            {
                throw new TimeoffInvalidDateIntervalException(command.RequestStartDate, command.RequestEndDate);
            }

            if(command.Author is null || (!command.Author.IsAdministrator))
            {
                command.EmployeeId = command.Author.Id;
            }

            Employee employee = await GetEmployee(command.EmployeeId);

            var timeoff = employee.CreateTimeoff(command.RequestStartDate, command.RequestEndDate, command.EmployeeComment);
            timeoff.UpdateMetadataOnModification(command.Author?.Id);
            
            this.RelatedAuditableEntity = timeoff;

            var commandContext = new Dictionary<string, object>() { 
                { "Employee", employee },
                { "Timeoff", timeoff }
            };
            command.Entries?.ToList().ForEach(async entryCommand => await _dispatcher.RunSubCommand(entryCommand, commandContext, command.Author, token));

            var events = employee.GetDomainEvents();
            employee.ClearDomainEvents();

            return events;
        }
    }
}

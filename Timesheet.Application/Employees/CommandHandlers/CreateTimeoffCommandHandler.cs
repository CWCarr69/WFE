using Timesheet.Application.Employees.Commands;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class CreateTimeoffCommandHandler : BaseSubCommandHandler<TimeoffHeader, CreateTimeoff>
    {
        private readonly IReadRepository<Employee> _readRepository;
        private readonly IDispatcher _dispatcher;

        public CreateTimeoffCommandHandler(
            IAuditHandler auditHandler,
            IReadRepository<Employee> readRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork
            ) : base(auditHandler, dispatcher, unitOfWork)
        {
            _readRepository = readRepository;
            _dispatcher = dispatcher;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCore(CreateTimeoff command, CancellationToken token)
        {
            if (command.RequestEndDate > command.RequestStartDate)
            {
                throw new TimeoffInvalidDateIntervalException(command.RequestStartDate, command.RequestEndDate);
            }

            Employee employee = await GetEmployee(command.EmployeeId);

            var timeoff = employee.CreateTimeoff(command.RequestStartDate, command.RequestEndDate, command.EmployeeComment, command.ApproverComment);
            this.RelatedAuditableEntity = timeoff;

            var commandContext = new Dictionary<string, object>() { { "Employee", employee } };
            command.Entries?.ToList().ForEach(async entryCommand => await _dispatcher.RunSubCommand(entryCommand, commandContext, token));

            return employee.GetDomainEvents();
        }

        private async Task<Employee> GetEmployee(string employeeId)
        {
            var employee = await _readRepository.Get(employeeId);

            if (employee is null)
            {
                throw new EntityNotFoundException<Employee>(employee?.Id);
            }

            return employee;
        }
    }
}

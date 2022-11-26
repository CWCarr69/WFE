using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Shared;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class AddEntryToTimeoffCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, AddEntryToTimeoff>
    {
        private readonly IEmployeeReadRepository _readRepository;
        private readonly IWorkflowService _workflowService;

        public AddEntryToTimeoffCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations) : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._readRepository = readRepository;
            this._workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(AddEntryToTimeoff command, CancellationToken token)
        {
            var employee = await GetEmployee(command);
            var timeoff = GetTimeoff(employee, command);


            this.RelatedAuditableEntity = LaunchedAsSubCommand ? null : timeoff;

            EmployeeRoleOnData currentEmployeeRoleOnData = GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.ADD_ENTRY, timeoff.Status, currentEmployeeRoleOnData);

            employee.AddTimeoffEntry(command.RequestDate, command.Type, command.Hours, timeoff, command.Label);
            timeoff.UpdateMetadataOnModification(command.Author?.Id);

            return Enumerable.Empty<IDomainEvent>();
        }

        private async Task<Employee> GetEmployee(AddEntryToTimeoff command)
        {
            if (LaunchedAsSubCommand)
            {
                var employee = _parentCommandContext["Employee"] as Employee;
                if(employee is null)
                {
                    throw new InvalidOperationException($"Cannot call subcommand {nameof(AddEntryToTimeoff)} without providing Employee data ");
                }
                return employee;
            }
            else
            {
                return await RequireEmployee(command.EmployeeId);
            }
        }

        private TimeoffHeader GetTimeoff(Employee employee, AddEntryToTimeoff command)
        {
            if (LaunchedAsSubCommand)
            {
                var timeoff = _parentCommandContext["Timeoff"] as TimeoffHeader;
                if (timeoff is null)
                {
                    throw new InvalidOperationException($"Cannot call subcommand {nameof(AddEntryToTimeoff)} without providing Timeoff data");
                }

                return timeoff;
            }
            else
            {
                return RequireTimeoff(employee, command.TimeoffId);
            }
        }
    }
}

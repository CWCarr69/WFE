using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class UpdateTimeoffEntryCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, UpdateTimeoffEntry>
    {
        private readonly IEmployeeReadRepository _readRepository;
        private readonly IWorkflowService _workflowService;

        public UpdateTimeoffEntryCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork) : base(auditHandler, readRepository, dispatcher, unitOfWork)
        {
            this._readRepository = readRepository;
            this._workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(UpdateTimeoffEntry command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);
            var timeoff = GetTimeoffOrThrowException(employee, command.TimeoffId);
            var timeoffEntry = GetTimeoffEntryOrThrowException(employee, timeoff, command.TimeoffEntryId);

            this.RelatedAuditableEntity = timeoff;

            EmployeeRoleOnData currentEmployeeRoleOnData = await GetCurrentEmployeeRoleOnData(command, employee);

            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.UPDATE_ENTRY, timeoff.Status, currentEmployeeRoleOnData);
            _workflowService.AuthorizeTransition(timeoffEntry, TimeoffEntryTransitions.UPDATE, timeoffEntry.Status, currentEmployeeRoleOnData);

            employee.UpdateTimeoffEntry(timeoff, timeoffEntry, command.Type, command.Hours);

            return LaunchedAsSubCommand ? Enumerable.Empty<IDomainEvent>() : employee.GetDomainEvents();
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
                return await GetEmployee(command.EmployeeId);
            }
        }

        private string GetTimeoffId(AddEntryToTimeoff command)
        {
            if (LaunchedAsSubCommand)
            {
                var timeoffId = _parentCommandContext["TimeoffId"] as string;
                if (timeoffId is null)
                {
                    throw new InvalidOperationException($"Cannot call subcommand {nameof(AddEntryToTimeoff)} without providing TimesheetId data");
                }

                return timeoffId;
            }
            else
            {
                return command.TimeoffId;
            }
        }
    }
}

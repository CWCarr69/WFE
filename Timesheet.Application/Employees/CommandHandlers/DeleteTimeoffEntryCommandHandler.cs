using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class DeleteTimeoffEntryCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, DeleteTimeoffEntry>
    {
        private readonly IWorkflowService _workflowService;

        public DeleteTimeoffEntryCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations
            ) : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            _workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(DeleteTimeoffEntry command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);
            var timeoff = GetTimeoffOrThrowException(employee, command.TimeoffId);
            var timeoffEntry = GetTimeoffEntryOrThrowException(employee, timeoff, command.TimeoffEntryId);

            this.RelatedAuditableEntity = timeoff;

            EmployeeRoleOnData currentEmployeeRoleOnData = await GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.DELETE_ENTRY, timeoff.Status, currentEmployeeRoleOnData);
            _workflowService.AuthorizeTransition(timeoffEntry, TimeoffEntryTransitions.DELETE, timeoffEntry.Status, currentEmployeeRoleOnData);

            employee.DeleteTimeoffEntry(timeoff, timeoffEntry);
            timeoff.UpdateMetadataOnModification(command.Author?.Id);

            return employee.GetDomainEvents();
        }
    }
}

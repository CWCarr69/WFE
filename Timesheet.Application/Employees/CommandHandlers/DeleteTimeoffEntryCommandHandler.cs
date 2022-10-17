using Timesheet.Application.Employees.Commands;
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
            IUnitOfWork unitOfWork
            ) : base(auditHandler, readRepository, dispatcher, unitOfWork)
        {
            _workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(DeleteTimeoffEntry command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);
            var timeoff = GetTimeoffOrThrowException(employee, command.TimeoffId);
            var timeoffEntry = GetTimeoffEntryOrThrowException(employee, timeoff, command.TimeoffEntryId);

            this.RelatedAuditableEntity = timeoff;

            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.DELETE_ENTRY, timeoff.Status);
            _workflowService.AuthorizeTransition(timeoffEntry, TimeoffEntryTransitions.DELETE, timeoffEntry.Status);

            employee.DeleteTimeoffEntry(timeoff, timeoffEntry);

            return employee.GetDomainEvents();
        }
    }
}

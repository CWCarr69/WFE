using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class ApproveTimeoffCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, ApproveTimeoff>
    {
        private readonly IWorkflowService _workflowService;

        public ApproveTimeoffCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository, 
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations) : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._workflowService = workflowService;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(ApproveTimeoff command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);

            var timeoff = GetTimeoffOrThrowException(employee, command.TimeoffId);
            this.RelatedAuditableEntity = timeoff;

            EmployeeRoleOnData currentEmployeeRoleOnData = await GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.APPROVE, timeoff.Status, currentEmployeeRoleOnData);

            employee.ApproveTimeoff(timeoff, command.Comment);
            timeoff.UpdateMetadataOnModification(command.Author?.Id);

            var events = employee.GetDomainEvents();
            employee.ClearDomainEvents();

            return events;
        }
    }
}

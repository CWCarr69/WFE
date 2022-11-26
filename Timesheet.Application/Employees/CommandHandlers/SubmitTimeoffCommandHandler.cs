using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Shared;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class SubmitTimeoffCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, SubmitTimeoff>
    {
        private readonly IWorkflowService _workflowService;

        public SubmitTimeoffCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository, 
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations) : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._workflowService = workflowService;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(SubmitTimeoff command, CancellationToken token)
        {
            var employee = await RequireEmployee(command.EmployeeId);
            var timeoff = RequireTimeoff(employee, command.TimeoffId);

            this.RelatedAuditableEntity = timeoff;

            EmployeeRoleOnData currentEmployeeRoleOnData = GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.SUBMIT, timeoff.Status, currentEmployeeRoleOnData);

            employee.SubmitTimeoff(timeoff, command.Comment);
            timeoff.UpdateMetadataOnModification(command.Author?.Id);

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

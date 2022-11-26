using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Shared;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class RejectTimeoffCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, RejectTimeoff>
    {
        private readonly IWorkflowService _workflowService;

        public RejectTimeoffCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations) : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(RejectTimeoff command, CancellationToken token)
        {
            var employee = await RequireEmployee(command.EmployeeId);
            var timeoff = RequireTimeoff(employee, command.TimeoffId);

            EmployeeRoleOnData currentEmployeeRoleOnData = GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.REJECT, timeoff.Status, currentEmployeeRoleOnData);

            employee.RejectTimeoff(timeoff, command.Comment);
            timeoff.UpdateMetadataOnModification(command.Author?.Id);

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

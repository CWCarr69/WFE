using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Shared;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class DeleteTimeoffCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, DeleteTimeoff>
    {
        private readonly IWorkflowService _workflowService;

        public DeleteTimeoffCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,

            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations) : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            _workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(DeleteTimeoff command, CancellationToken token)
        {
            var employee = await RequireEmployee(command.EmployeeId);

            var timeoff = RequireTimeoff(employee, command.TimeoffId);
            this.RelatedAuditableEntity = timeoff;

            EmployeeRoleOnData currentEmployeeRoleOnData = GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.APPROVE, timeoff.Status, currentEmployeeRoleOnData);

            employee.DeleteTimeoff(timeoff);

            return employee.GetDomainEvents();
        }
    }
}

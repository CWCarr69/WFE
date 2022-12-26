using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Shared;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class UpdateTimeoffCommentCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, UpdateTimeoffComment>
    {
        private readonly IWorkflowService _workflowService;

        public UpdateTimeoffCommentCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,

            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations) : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            _workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(UpdateTimeoffComment command, CancellationToken token)
        {
            var employee = await RequireEmployee(command.EmployeeId);

            var timeoff = RequireTimeoff(employee, command.TimeoffId);
            this.RelatedAuditableEntity = timeoff;

            EmployeeRoleOnData currentEmployeeRoleOnData = GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.UPDATE_COMMENT, timeoff.Status, currentEmployeeRoleOnData);

            if(command.ApproverComment!= null)
            {
                employee.AddApproverComment(timeoff, command.ApproverComment);
            }

            if (command.EmployeeComment != null)
            {
                employee.AddComment(timeoff, command.EmployeeComment);
            }

            return employee.GetDomainEvents();
        }
    }
}

using System.Threading;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Timesheets.Commands;
using Timesheet.Application.TImesheets.CommandHandlers;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.CommandHandlers
{
    internal class UpdateTimesheetCommentCommandHandler : BaseTimesheetCommandHandler<TimesheetHeader, UpdateTimesheetComment>
    {
        private readonly IWorkflowService _workflowService;

        public UpdateTimesheetCommentCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository employeeReadRepository,
            ITimesheetReadRepository readRepository, 
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations
            )
            : base(auditHandler, employeeReadRepository, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._workflowService = workflowService;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(UpdateTimesheetComment command, CancellationToken token)
        {
            var employee = await RequireEmployee(command.EmployeeId);

            var timesheet = await RequireTimesheet(command.TimesheetId, employee?.Id);
            this.RelatedAuditableEntity = timesheet;

            var timesheetEntryRef = GetTimesheetFirstData(timesheet);

            EmployeeRoleOnData currentEmployeeRoleOnData = GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timesheet, TimesheetTransitions.APPROVE, timesheet.Status, currentEmployeeRoleOnData);
            if (timesheetEntryRef is not null)
            {
                _workflowService.AuthorizeTransition(timesheetEntryRef, TimesheetEntryTransitions.APPROVE, timesheetEntryRef.Status, currentEmployeeRoleOnData);
            }

            if (command.ApproverComment != null)
            {
                timesheet.AddApproverComment(employee, command.ApproverComment);
            }

            if (command.EmployeeComment != null)
            {
                timesheet.AddComment(employee, command.EmployeeComment);
            }

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

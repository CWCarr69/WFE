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
    internal class FinalizeTimesheetCommandHandler : BaseTimesheetCommandHandler<TimesheetHeader, FinalizeTimesheet>
    {
        private readonly IWorkflowService _workflowService;
        private readonly IUnitOfWork _unitOfWork;

        public FinalizeTimesheetCommandHandler(
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
            this._unitOfWork = unitOfWork;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(FinalizeTimesheet command, CancellationToken token)
        {

            var timesheet = await RequireTimesheet(command.TimesheetId);
            this.RelatedAuditableEntity = timesheet;

            EmployeeRoleOnData currentEmployeeRoleOnData = await GetCurrentEmployeeRoleOnData(command, null);
            _workflowService.AuthorizeTransition(timesheet, TimesheetTransitions.FINALIZE, timesheet.Status, currentEmployeeRoleOnData);

            timesheet.FinalizeTimesheet();

            await _unitOfWork.CompleteAsync(token); // COMPLETE TRANSACTION BEFORE PROCESSING EVENT AS FINALIZING TIMESHEET IS ATOMIC

            var events = timesheet.GetDomainEvents();
            timesheet.ClearDomainEvents();

            return events;

        }
    }
}

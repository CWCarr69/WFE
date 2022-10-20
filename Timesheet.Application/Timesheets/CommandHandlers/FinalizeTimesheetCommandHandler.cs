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

        public FinalizeTimesheetCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository employeeReadRepository,
            ITimesheetReadRepository readRepository, 
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork)
            : base(auditHandler, employeeReadRepository, readRepository, dispatcher, unitOfWork)
        {
            this._workflowService = workflowService;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(FinalizeTimesheet command, CancellationToken token)
        {

            var timesheet = await GetTimesheetOrThrowException(command.TimesheetId);
            this.RelatedAuditableEntity = timesheet;

            EmployeeRoleOnData currentEmployeeRoleOnData = await GetCurrentEmployeeRoleOnData(command, null);
            _workflowService.AuthorizeTransition(timesheet, TimesheetTransitions.FINALIZE, timesheet.Status, currentEmployeeRoleOnData);

            timesheet.Finalize();

            var events = timesheet.GetDomainEvents();
            timesheet.ClearDomainEvents();

            return events;

        }
    }
}

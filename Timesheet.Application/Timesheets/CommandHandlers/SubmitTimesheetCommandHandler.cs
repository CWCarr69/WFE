using Timesheet.Application.Timesheets.Commands;
using Timesheet.Application.TImesheets.CommandHandlers;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.CommandHandlers
{
    internal class SubmitTimesheetCommandHandler : BaseTimesheetCommandHandler<TimesheetHeader, SubmitTimesheet>
    {
        private readonly IWorkflowService _workflowService;

        public SubmitTimesheetCommandHandler(
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

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(SubmitTimesheet command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);

            var timesheet = await GetTimesheetOrThrowException(command.TimesheetId, employee?.Id);
            this.RelatedAuditableEntity = timesheet;

            var timesheetEntryRef = GetTimesheetFirstData(timesheet);

            EmployeeRoleOnData currentEmployeeRoleOnData = await GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timesheet, TimesheetTransitions.SUBMIT, timesheet.Status, currentEmployeeRoleOnData);
            _workflowService.AuthorizeTransition(timesheetEntryRef, TimesheetEntryTransitions.SUBMIT, timesheetEntryRef.Status, currentEmployeeRoleOnData);

            timesheet.Submit(employee);

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

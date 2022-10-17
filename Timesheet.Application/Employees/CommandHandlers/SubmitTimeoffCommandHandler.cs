using Timesheet.Application.Employees.Commands;
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
            IUnitOfWork unitOfWork) : base(auditHandler, readRepository, dispatcher, unitOfWork)
        {
            this._workflowService = workflowService;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(SubmitTimeoff command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);
            var timeoff = GetTimeoffOrThrowException(employee, command.TimeoffId);

            this.RelatedAuditableEntity = timeoff;

            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.SUBMIT, timeoff.Status);

            employee.SubmitTimeoff(timeoff, command.Comment);

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

using Timesheet.Application.Employees.Commands;
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
            IReadRepository<Employee> readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork) : base(auditHandler, readRepository, dispatcher, unitOfWork)
        {
            this._workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCore(RejectTimeoff command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);
            var timeoff = GetTimeoffOrThrowException(employee, command.TimeoffId);

            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.REJECT, timeoff.Status);

            employee.RejectTimeoff(timeoff, command.Comment);

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Workflow;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class ApproveTimeoffCommandHandler : BaseEmployeeCommandHandler<ApproveTimeoff>
    {
        private readonly IWorkflowService _workflowService;

        public ApproveTimeoffCommandHandler(
            IReadRepository<Employee> readRepository, 
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork) : base(readRepository, dispatcher, unitOfWork)
        {
            this._workflowService = workflowService;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCore(ApproveTimeoff command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);
            var timeoff = GetTimeoffOrThrowException(employee, command.TimeoffId);

            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.APPROVE, timeoff.Status);

            employee.ApproveTimeoff(timeoff, command.Comment);

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

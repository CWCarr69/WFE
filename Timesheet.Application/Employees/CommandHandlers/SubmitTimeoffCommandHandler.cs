using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Workflow;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class SubmitTimeoffCommandHandler : BaseEmployeeCommandHandler<SubmitTimeoff>
    {
        private readonly IWorkflowService _workflowService;

        public SubmitTimeoffCommandHandler(
            IReadRepository<Employee> readRepository, 
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork) : base(readRepository, dispatcher, unitOfWork)
        {
            this._workflowService = workflowService;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCore(SubmitTimeoff command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);
            var timeoff = GetTimeoffOrThrowException(employee, command.TimeoffId);

            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.SUBMIT, timeoff.Status);

            employee.SubmitTimeoff(timeoff, command.Comment);

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

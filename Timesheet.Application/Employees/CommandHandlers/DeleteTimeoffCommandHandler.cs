using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class DeleteTimeoffCommandHandler : BaseEmployeeCommandHandler<TimeoffHeader, DeleteTimeoff>
    {
        private readonly IReadRepository<Employee> _readRepository;
        private readonly IWorkflowService _workflowService;

        public DeleteTimeoffCommandHandler(
            IAuditHandler auditHandler,
            IReadRepository<Employee> readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork
            ) : base(auditHandler, readRepository, dispatcher, unitOfWork)
        {
            _readRepository = readRepository;
            _workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCore(DeleteTimeoff command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);

            var timeoff = GetTimeoffOrThrowException(employee, command.TimeoffId);
            this.RelatedAuditableEntity = timeoff;

            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.APPROVE, timeoff.Status);

            employee.DeleteTimeoff(timeoff);

            return employee.GetDomainEvents();
        }
    }
}

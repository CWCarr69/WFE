using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class ModifyApproverCommandHandler : BaseEmployeeCommandHandler<Employee, ModifyApprover>
    {
        private readonly IReadRepository<Employee> _readRepository;
        private readonly IWorkflowService _workflowService;

        public ModifyApproverCommandHandler(
            IAuditHandler auditHandler,
            IReadRepository<Employee> readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork) : base(auditHandler, readRepository, dispatcher, unitOfWork)
        {
            this._readRepository = readRepository;
            this._workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCore(ModifyApprover command, CancellationToken token)
        {
            var employee = await GetEmployee(command.EmployeeId);

            this.RelatedAuditableEntity = employee;

            if (command.PrimaryApproverId is not null)
            {
                var primaryApprover = await GetEmployee(command.PrimaryApproverId);
                employee.SetPrimaryApprover(primaryApprover);
            }

            if(command.SecondaryApproverId is not null)
            {
                var secondaryApprover = await GetEmployee(command.SecondaryApproverId);

                employee.SetSecondaryOfficer(secondaryApprover);
            }

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

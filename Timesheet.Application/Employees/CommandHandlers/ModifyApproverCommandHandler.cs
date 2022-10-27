using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class ModifyApproverCommandHandler : BaseEmployeeCommandHandler<Employee, ModifyApprover>
    {

        public ModifyApproverCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations) 
            : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(ModifyApprover command, CancellationToken token)
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

            employee.UpdateMetadataOnModification(command.Author?.Id);

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

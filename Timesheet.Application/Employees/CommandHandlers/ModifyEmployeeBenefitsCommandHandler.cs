using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Shared;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class ModifyEmployeeBenefitsCommandHandler : BaseEmployeeCommandHandler<Employee, ModifyEmployeeBenefits>
    {

        public ModifyEmployeeBenefitsCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations) 
            : base(auditHandler, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(ModifyEmployeeBenefits command, CancellationToken token)
        {
            var employee = await RequireEmployee(command.EmployeeId);

            this.RelatedAuditableEntity = employee;

            employee.ChangeBenefitsCalculationMode(command.ConsiderFixedBenefits);
            employee.SetBenefits(command.VacationHours, command.PersonalHours, command.RolloverHours);

            employee.UpdateMetadataOnModification(command.Author?.Id);

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

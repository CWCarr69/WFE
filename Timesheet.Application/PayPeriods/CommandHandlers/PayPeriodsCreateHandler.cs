using Timesheet.Application.Employees.Services;
using Timesheet.Application.PayPeriod.Commands;
using Timesheet.Application.Shared;
using Timesheet.Domain;
using Timesheet.Domain.Models.PayPeriod;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.PayPeriods.CommandHandlers
{
  internal class PayPeriodsCreateHandler : BaseCommandHandler<PayPeriodCreate, PayPeriodCreateCommand>
  {
    private readonly IPayPeriodRepository _payPeriodRepository;

    public PayPeriodsCreateHandler(
            IEmployeeReadRepository employeeReadRepository,
            IAuditHandler auditHandler,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IPayPeriodRepository payPeriodRepository,
            IEmployeeHabilitation employeeHabilitation)
      : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitation)
    {
      _payPeriodRepository = payPeriodRepository;
    }
    public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(PayPeriodCreateCommand command, CancellationToken token)
    {
      var result = await _payPeriodRepository.PayPeriodCreateAsync(command.StartFrom, token);

      command.Result = result; // Store result in command

      return Enumerable.Empty<IDomainEvent>();
    }
  }
}

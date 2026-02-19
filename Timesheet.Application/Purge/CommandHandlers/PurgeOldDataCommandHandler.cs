using Timesheet.Application.Employees.Services;
using Timesheet.Application.Purge.Commands;
using Timesheet.Application.Shared;
using Timesheet.Domain;
using Timesheet.Domain.Models.Purge;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Purge.CommandHandlers
{
  internal class PurgeOldDataCommandHandler : BaseCommandHandler<PurgeOldData, PurgeOldDataCommand>
  {
    private readonly IPurgeOldDataRepository _purgeOldRepository;

    public PurgeOldDataCommandHandler(
            IEmployeeReadRepository employeeReadRepository,
            IAuditHandler auditHandler,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IPurgeOldDataRepository purgeOldRepository,
            IEmployeeHabilitation employeeHabilitation)
      : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitation)
    {
      _purgeOldRepository = purgeOldRepository;
    }

    public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(PurgeOldDataCommand command, CancellationToken token)
    {
      var result = await _purgeOldRepository.PurgeOldDataAsync(command.OlderThan, token);

      command.Result = result; // Store result in command

      return Enumerable.Empty<IDomainEvent>();
    }
  }
}

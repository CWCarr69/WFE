using Timesheet.Application.Employees.Services;
using Timesheet.Application.Purge.Commands;
using Timesheet.Application.Shared;
using Timesheet.Domain;
using Timesheet.Domain.Models;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Repositories;
using PurgeOldData = Timesheet.Domain.Models.Purge.PurgeOldData;

namespace Timesheet.Application.Purge.CommandHandlers
{
  internal class PurgeOldDataCommandHandler : BaseCommandHandler<PurgeOldData , PurgeOldDataCommand>
  {
    private readonly IPurgeOldDataRepository _purgeOldRepository;

    public PurgeOldDataCommandHandler(
            IEmployeeReadRepository employeeReadRepository,
            IAuditHandler auditHandler, 
            IDispatcher dispatcher, 
            IUnitOfWork unitOfWork,
            IPurgeOldDataRepository purgeOldRepository,
            IEmployeeHabilitation employeeHabilitation)
      : base(employeeReadRepository,auditHandler,dispatcher, unitOfWork,employeeHabilitation)
    {
      _purgeOldRepository = purgeOldRepository;
    }

    public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(PurgeOldDataCommand command, CancellationToken token)
    {
      _purgeOldRepository.PurgeOldDataAsync(command.OlderThan, token);

      return Enumerable.Empty<IDomainEvent>();
    }
  }
}

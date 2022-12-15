using Timesheet.Application.Employees.Services;
using Timesheet.Application.Settings.Commands;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Settings;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Settings.CommandHandlers
{
    internal class UpdateSettingCommandHandler : BaseCommandHandler<Setting, UpdateSetting>
    {
        public readonly IWriteRepository<Setting> _writeRepository;
        public readonly IReadRepository<Setting> _readRepository;

        public UpdateSettingCommandHandler(
            IEmployeeReadRepository employeeReadRepository,
            IEmployeeHabilitation employeeHabilitations,
            IAuditHandler auditHandler,
            IWriteRepository<Setting> writeRepository,
            IReadRepository<Setting> readRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork
            ) : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitations)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(UpdateSetting updateSetting, CancellationToken token)
        {
            if (updateSetting.Id == null)
            {
                throw new EntityNotFoundException<Setting>(updateSetting.Id);
            }

            var existingSetting = await _readRepository.Get(updateSetting.Id);
            if (existingSetting is null)
            {
                throw new EntityNotFoundException<Setting>(updateSetting.Id);
            }

            existingSetting.UpdateValue(updateSetting.Value);

            this.RelatedAuditableEntity = existingSetting;

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

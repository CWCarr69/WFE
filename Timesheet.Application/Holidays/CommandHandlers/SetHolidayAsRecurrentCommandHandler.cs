using Timesheet.Application.Holidays.Commands;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.CommandHandlers
{
    internal class SetHolidayAsRecurrentCommandHandler : BaseCommandHandler<Holiday, SetHolidayAsRecurrent>
    {
        public readonly IWriteRepository<Holiday> _writeRepository;
        public readonly IHolidayReadRepository _readRepository;

        public SetHolidayAsRecurrentCommandHandler(
            IAuditHandler auditHandler,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeReadRepository employeeReadRepository,
            IWriteRepository<Holiday> writeRepository,
            IHolidayReadRepository readRepository) : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(SetHolidayAsRecurrent setHolidayAsRecurrent, CancellationToken token)
        {
            if (setHolidayAsRecurrent.Id is null)
            {
                throw new EntityNotFoundException<Holiday>(setHolidayAsRecurrent.Id);
            }

            var existingHoliday = await _readRepository.Get(setHolidayAsRecurrent.Id);
            if (existingHoliday is null)
            {
                throw new EntityNotFoundException<Holiday>(setHolidayAsRecurrent.Id);
            }

            existingHoliday.SetAsRecurrent();
            await _writeRepository.Add(existingHoliday);

            this.RelatedAuditableEntity = existingHoliday;

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

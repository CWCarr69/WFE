using Timesheet.Application.Holidays.Commands;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.CommandHandlers
{
    internal class DeleteHolidayCommandHandler : BaseCommandHandler<Holiday, DeleteHoliday>
    {
        public readonly IWriteRepository<Holiday> _writeRepository;
        public readonly IHolidayReadRepository _readRepository;

        public DeleteHolidayCommandHandler(
            IAuditHandler auditHandler,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IWriteRepository<Holiday> writeRepository,
            IHolidayReadRepository readRepository) : base(auditHandler, dispatcher, unitOfWork)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(DeleteHoliday deleteHoliday, CancellationToken token)
        {
            if (deleteHoliday.Id is null)
            {
                throw new EntityNotFoundException<Holiday>(deleteHoliday.Id);
            }

            var holiday = await _readRepository.Get(deleteHoliday.Id);

            if (holiday is null)
            {
                throw new EntityNotFoundException<Holiday>(deleteHoliday.Id);
            }

            this._writeRepository.Delete(deleteHoliday.Id);

            this.RelatedAuditableEntity = holiday;

            return holiday.GetDomainEvents();
        }
    }
}

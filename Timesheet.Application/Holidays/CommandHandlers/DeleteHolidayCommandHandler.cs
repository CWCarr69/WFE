using Timesheet.Application.Holidays.Commands;
using Timesheet.Application.Queries;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.CommandHandlers
{
    internal class DeleteHolidayCommandHandler : BaseCommandHandler<DeleteHoliday>
    {
        public readonly IWriteRepository<Holiday> _writeRepository;
        public readonly IHolidayQuery _readRepository;

        public DeleteHolidayCommandHandler(IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IWriteRepository<Holiday> writeRepository,
            IHolidayQuery readRepository) : base(dispatcher, unitOfWork)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCore(DeleteHoliday deleteHoliday, CancellationToken token)
        {
            if (deleteHoliday.Id is null)
            {
                throw new EntityNotFoundException<Holiday>(deleteHoliday.Id);
            }

            var holiday = await _readRepository.Get(deleteHoliday.Id);

            if (holiday is not null)
            {
                throw new EntityNotFoundException<Holiday>(deleteHoliday.Id);
            }

            this._writeRepository.Delete(deleteHoliday.Id);

            return holiday.GetDomainEvents();
        }
    }
}

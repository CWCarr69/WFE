using Timesheet.Application.Holidays.Commands;
using Timesheet.Application.Queries;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.CommandHandlers
{
    internal class SetHolidayAsRecurrentCommandHandler : BaseEmployeeCommandHandler<SetHolidayAsRecurrent>
    {
        public readonly IWriteRepository<Holiday> _writeRepository;
        public readonly IQueryHoliday _readRepository;

        public SetHolidayAsRecurrentCommandHandler(IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IWriteRepository<Holiday> writeRepository,
            IQueryHoliday readRepository) : base(dispatcher, unitOfWork)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCore(SetHolidayAsRecurrent setHolidayAsRecurrent, CancellationToken token)
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

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

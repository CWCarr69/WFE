using Timesheet.Application.Holidays.Commands;
using Timesheet.Application.Queries;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.CommandHandlers
{
    internal class AddHolidayCommandHandler : BaseCommandHandler<AddHoliday>
    {
        public readonly IWriteRepository<Holiday> _writeRepository;
        public readonly IHolidayQuery _readRepository;

        public AddHolidayCommandHandler(IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IWriteRepository<Holiday> writeRepository,
            IHolidayQuery readRepository) : base(dispatcher, unitOfWork)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCore(AddHoliday addHoliday, CancellationToken token)
        {
            if (_readRepository.GetByDate(addHoliday.Date) is not null)
            {
                throw new HolidayAlreadyExistException(addHoliday.Date);
            }

            var holiday = Holiday.Create(addHoliday.Date, addHoliday.Description, addHoliday.Notes, addHoliday.IsRecurrent);
            await _writeRepository.Add(holiday);

            return holiday.GetDomainEvents();
        }
    }
}

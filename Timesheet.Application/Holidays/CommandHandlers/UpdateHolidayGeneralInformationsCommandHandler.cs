using Timesheet.Application.Holidays.Commands;
using Timesheet.Application.Queries;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.CommandHandlers
{
    internal class UpdateHolidayGeneralInformationsCommandHandler : BaseEmployeeCommandHandler<UpdateHolidayGeneralInformations>
    {
        public readonly IWriteRepository<Holiday> _writeRepository;
        public readonly IQueryHoliday _readRepository;

        public UpdateHolidayGeneralInformationsCommandHandler(IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IWriteRepository<Holiday> writeRepository,
            IQueryHoliday readRepository) : base(dispatcher, unitOfWork)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCore(UpdateHolidayGeneralInformations updateHolidayGeneralInformations, CancellationToken token)
        {
            if (updateHolidayGeneralInformations.Id == null)
            {
                throw new EntityNotFoundException<Holiday>(updateHolidayGeneralInformations.Id);
            }

            var existingHoliday = await _readRepository.Get(updateHolidayGeneralInformations.Id);
            if (existingHoliday is null)
            {
                throw new EntityNotFoundException<Holiday>(updateHolidayGeneralInformations.Id);
            }

            existingHoliday.UpdateInformations(updateHolidayGeneralInformations.Description, updateHolidayGeneralInformations.Notes); ;
            await _writeRepository.Add(existingHoliday);

            return existingHoliday.GetDomainEvents();
        }
    }
}

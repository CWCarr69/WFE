using Timesheet.Application.Employees.Services;
using Timesheet.Application.Holidays.Commands;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Holidays;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Holidays.CommandHandlers
{
    internal class UpdateHolidayGeneralInformationsCommandHandler : BaseCommandHandler<Holiday, UpdateHolidayGeneralInformations>
    {
        public readonly IWriteRepository<Holiday> _writeRepository;
        public readonly IHolidayReadRepository _readRepository;

        public UpdateHolidayGeneralInformationsCommandHandler(
            IEmployeeReadRepository employeeReadRepository,
            IAuditHandler auditHandler,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IWriteRepository<Holiday> writeRepository,
            IHolidayReadRepository readRepository,
            IEmployeeHabilitation employeeHabilitation
            )
            : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitation)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCoreAsync(UpdateHolidayGeneralInformations updateHolidayGeneralInformations, CancellationToken token)
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


            this.RelatedAuditableEntity = existingHoliday;

            var events = existingHoliday.GetDomainEvents();
            existingHoliday.ClearDomainEvents();

            return events;
        }
    }
}

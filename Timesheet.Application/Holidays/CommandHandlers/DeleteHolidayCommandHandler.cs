using Timesheet.Application.Employees.Services;
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
            IEmployeeReadRepository employeeReadRepository,
            IWriteRepository<Holiday> writeRepository,
            IHolidayReadRepository readRepository,
            IEmployeeHabilitation employeeHabilitation
            ) : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitation)
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

            var events = holiday.GetDomainEvents();
            holiday.ClearDomainEvents();

            return events;
        }
    }
}

using Timesheet.Application.Employees.Services;
using Timesheet.Application.Timesheets.Commands;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.CommandHandlers
{
    internal class AddTimesheetExceptionCommandHandler : BaseCommandHandler<TimesheetException, AddTimesheetException>
    {
        private readonly ITimesheetReadRepository _readRepository;
        private readonly IWriteRepository<TimesheetException> _writeRepository;
        private readonly ITimesheetReadRepository _timesheetReadRepository;

        public AddTimesheetExceptionCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository employeeReadRepository,
            ITimesheetReadRepository timesheetReadRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations
            )
            : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._timesheetReadRepository = timesheetReadRepository;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(AddTimesheetException command, CancellationToken token)
        {
            var timesheet = await _timesheetReadRepository.GetTimesheetWithEntries(command.TimesheetId);
            if (timesheet is null)
            {
                throw new EntityNotFoundException<TimesheetHeader>(command.TimesheetId);
            }

            timesheet.AddTimesheetException(command.TimesheetEntryId, command.EmployeeId, command.IsHoliday);

            var timesheetException = new TimesheetException(command.TimesheetEntryId, command.EmployeeId, command.IsHoliday ? nameof(Domain.Models.Holidays.Holiday) : nameof(Domain.Models.Timesheets));

            this.RelatedAuditableEntity = timesheetException;

            var events = timesheet.GetDomainEvents();
            timesheet.ClearDomainEvents();

            return events;
        }
    }
}

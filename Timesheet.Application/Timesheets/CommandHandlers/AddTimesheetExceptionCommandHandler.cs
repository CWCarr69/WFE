using Timesheet.Application.Employees.Services;
using Timesheet.Application.Timesheets.Commands;
using Timesheet.Domain;
using Timesheet.Domain.Repositories;
using Timesheet.Domain.Timesheets;

namespace Timesheet.Application.Timesheets.CommandHandlers
{
    internal class AddTimesheetExceptionCommandHandler : BaseCommandHandler<TimesheetException, AddTimesheetException>
    {
        private readonly ITimesheetReadRepository _readRepository;
        private readonly IWriteRepository<TimesheetException> _writeRepository;

        public AddTimesheetExceptionCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository employeeReadRepository,
            IWriteRepository<TimesheetException> writeRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations
            )
            : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._writeRepository= writeRepository;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(AddTimesheetException command, CancellationToken token)
        {
            var timesheetException = new TimesheetException(command.TimesheetEntryId, command.EmployeeId, command.IsHoliday ? nameof(Domain.Models.Holidays.Holiday) : nameof(Domain.Models.Timesheets));

            this.RelatedAuditableEntity = timesheetException;

            return Enumerable.Empty<IDomainEvent>();
        }
    }
}

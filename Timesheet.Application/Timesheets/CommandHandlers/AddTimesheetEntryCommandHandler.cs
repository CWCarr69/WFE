using Timesheet.Application.Employees.Services;
using Timesheet.Application.Timesheets.Commands;
using Timesheet.Application.TImesheets.CommandHandlers;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.CommandHandlers
{
    internal class AddTimesheetEntryCommandHandler : BaseTimesheetCommandHandler<TimesheetHeader, AddTimesheetEntry>
    {
        private readonly ITimesheetReadRepository _readRepository;
        private readonly IWriteRepository<TimesheetHeader> _writeRepository;

        public AddTimesheetEntryCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository employeeReadRepository,
            IWriteRepository<TimesheetHeader> writeRepository,
            ITimesheetReadRepository readRepository, 
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations
            )
            : base(auditHandler, employeeReadRepository, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._writeRepository= writeRepository;
            this._readRepository = readRepository;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(AddTimesheetEntry command, CancellationToken token)
        {
            var employee = await RequireEmployee(command.EmployeeId);

            var timesheetEntry = new TimesheetEntry(
                Entity.GenerateId(),
                command.EmployeeId,
                command.WorkDate.Date,
                command.PayrollCodeId,
                command.Quantity,
                command.Description,
                command.ServiceOrderNumber,
                command.ServiceOrderDescription,
                command.JobNumber,
                command.JobDescription,
                command.ProfitCenterNumber,
                command.OutOffCountry,
                true);

            var timesheet = employee.EmploymentData.IsSalaried
                    ? TimesheetHeader.CreateWeeklyTimesheet(command.WorkDate)
                    : TimesheetHeader.CreateMonthlyTimesheet(command.WorkDate);

            var alreadyAddedtimesheet = await _readRepository.GetTimesheet(timesheet.Id);

            if (alreadyAddedtimesheet is null)
            {
                await _writeRepository.Add(timesheet);
            }

            timesheet.AddTimesheetEntry(timesheetEntry);

            this.RelatedAuditableEntity = timesheet;

            var events = timesheet.GetDomainEvents();
            timesheet.ClearDomainEvents();

            return events;
        }
    }
}

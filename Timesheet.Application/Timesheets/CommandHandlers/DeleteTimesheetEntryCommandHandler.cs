using System.Threading;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Timesheets.Commands;
using Timesheet.Application.TImesheets.CommandHandlers;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.CommandHandlers
{
    internal class DeleteTimesheetEntryCommandHandler : BaseTimesheetCommandHandler<TimesheetHeader, DeleteTimesheetEntry>
    {
        private readonly ITimesheetReadRepository _readRepository;
        private readonly IWriteRepository<TimesheetHeader> _writeRepository;

        public DeleteTimesheetEntryCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository employeeReadRepository,
            IWriteRepository<TimesheetHeader> writeRepository,
            ITimesheetReadRepository readRepository, 
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations
            )
            : base(auditHandler, employeeReadRepository, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._writeRepository= writeRepository;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(DeleteTimesheetEntry command, CancellationToken token)
        {
            var timesheet = await _readRepository.GetTimesheetWithEntries(command.TimesheetId, command.EmployeeId);
            var timesheetEntry = timesheet != null
                ? timesheet.TimesheetEntries.FirstOrDefault(e => e.Id == command.TimesheetEntryId)
                : null;

            if (timesheetEntry is not null)
            {
                this.RelatedAuditableEntity = timesheet;
                timesheet.DeleteTimesheetEntry(timesheetEntry);
            }

            var events = timesheet.GetDomainEvents();
            timesheet.ClearDomainEvents();

            return events;
        }
    }
}

using Timesheet.Application.Employees.Services;
using Timesheet.Application.Shared;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.TImesheets.CommandHandlers
{
    internal abstract class BaseTimesheetCommandHandler<TEntity, TCommand> : BaseEmployeeCommandHandler<TEntity, TCommand> 
        where TEntity : Entity
        where TCommand : ICommand
    {
        private readonly IEmployeeReadRepository _employeeReadRepository;
        private readonly ITimesheetReadRepository _readRepository;

        protected BaseTimesheetCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository employeeReadRepository,
            ITimesheetReadRepository readRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations
            ) : base(auditHandler, employeeReadRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._employeeReadRepository = employeeReadRepository;
            this._readRepository = readRepository;
        }

        protected TimesheetEntry GetTimesheetFirstData(TimesheetHeader timesheet)
        {
            return timesheet.TimesheetEntriesWithoutTimeoffs
                .OrderBy(te => (int)te.Status)
                .FirstOrDefault(t => t.Status != TimesheetEntryStatus.APPROVED);
        }

        protected async Task<TimesheetHeader> RequireTimesheet(string timesheetId, string? employeeId=null)
        {
            var timesheet = await _readRepository.GetTimesheetWithEntries(timesheetId, employeeId);
            if (timesheet is null)
            {
                throw new EntityNotFoundException<TimesheetHeader>(timesheetId);
            }
            return timesheet;
        }
    }
}

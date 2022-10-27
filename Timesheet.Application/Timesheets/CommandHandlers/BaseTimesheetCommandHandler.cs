using Timesheet.Application.Employees.Services;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.TImesheets.CommandHandlers
{
    internal abstract class BaseTimesheetCommandHandler<TEntity, TCommand> : BaseCommandHandler<TEntity, TCommand> 
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
            ) : base(employeeReadRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._employeeReadRepository = employeeReadRepository;
            this._readRepository = readRepository;
        }

        protected async Task<Employee> GetEmployee(string employeeId)
        {
            var employee = await _employeeReadRepository.GetEmployee(employeeId);

            if (employee is null)
            {
                throw new EntityNotFoundException<Employee>(employee?.Id);
            }

            return employee;
        }

        protected TimesheetEntry GetTimesheetFirstData(TimesheetHeader timesheet)
        {
            var timesheetEntry = timesheet.TimesheetEntries
                .FirstOrDefault(t => t.PayrollCode != TimesheetPayrollCode.HOLIDAY.ToString());
            
            if (timesheetEntry is null)
            {
                throw new EntityNotFoundException<TimesheetHeader>(timesheet?.Id);
            }
            return timesheetEntry;
        }

        protected async Task<TimesheetHeader> GetTimesheetOrThrowException(string timesheetId, string? employeeId=null)
        {
            var timesheet = await _readRepository.GetTimesheet(timesheetId, employeeId);
            if (timesheet is null)
            {
                throw new EntityNotFoundException<TimesheetHeader>(timesheetId);
            }
            return timesheet;
        }
    }
}

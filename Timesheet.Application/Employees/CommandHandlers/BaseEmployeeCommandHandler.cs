using Timesheet.Application.Shared;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal abstract class BaseEmployeeCommandHandler<TEntity, TCommand> : BaseSubCommandHandler<TEntity, TCommand> 
        where TEntity : Entity
        where TCommand : ICommand
    {
        private readonly IEmployeeReadRepository _readRepository;

        protected BaseEmployeeCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository readRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork) : base(readRepository, auditHandler, dispatcher, unitOfWork)
        {
            this._readRepository = readRepository;
        }

        protected async Task<Employee> GetEmployee(string employeeId)
        {
            var employee = await _readRepository.GetEmployee(employeeId);

            if (employee is null)
            {
                throw new EntityNotFoundException<Employee>(employee?.Id);
            }

            return employee;
        }

        protected TimeoffEntry GetTimeoffEntryOrThrowException(Employee employee, TimeoffHeader timeoff, string timeoffEntryId)
        {
            var timeoffEntry = timeoff?.GetTimeoffEntry(timeoffEntryId);
            if (timeoffEntry is null)
            {
                throw new EntityNotFoundException<TimeoffEntry>(timeoffEntryId);
            }
            return timeoffEntry;
        }

        protected TimeoffHeader GetTimeoffOrThrowException(Employee employee, string timeoffId)
        {
            var timeoffEntry = employee.GetTimeoff(timeoffId);
            if (timeoffEntry is null)
            {
                throw new EntityNotFoundException<TimeoffHeader>(timeoffId);
            }
            return timeoffEntry;
        }
    }
}

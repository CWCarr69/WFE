using Timesheet.Application.Employees.Services;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Shared
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
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations)
            : base(readRepository, auditHandler, dispatcher, unitOfWork, employeeHabilitations)
        {
            _readRepository = readRepository;
        }

        protected async Task<Employee> RequireEmployee(string employeeId)
        {
            var employee = await _readRepository.GetEmployee(employeeId);

            if (employee is null)
            {
                throw new EntityNotFoundException<Employee>(employeeId);
            }

            return employee;
        }

        protected TimeoffEntry RequireTimeoffEntry(Employee employee, TimeoffHeader timeoff, string timeoffEntryId)
        {
            var timeoffEntry = timeoff?.GetTimeoffEntry(timeoffEntryId);
            if (timeoffEntry is null)
            {
                throw new EntityNotFoundException<TimeoffEntry>(timeoffEntryId);
            }
            return timeoffEntry;
        }

        protected TimeoffHeader RequireTimeoff(Employee employee, string timeoffId)
        {
            var timeoff = employee.GetTimeoff(timeoffId);
            if (timeoff is null)
            {
                throw new EntityNotFoundException<TimeoffHeader>(timeoffId);
            }
            return timeoff;
        }
    }
}

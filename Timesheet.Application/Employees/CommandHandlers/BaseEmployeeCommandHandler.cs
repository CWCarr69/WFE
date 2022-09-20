using Timesheet.Application.Workflow;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal abstract class BaseEmployeeCommandHandler<TCommand> : BaseSubCommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly IReadRepository<Employee> _readRepository;

        protected BaseEmployeeCommandHandler(
            IReadRepository<Employee> readRepository,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork) : base(dispatcher, unitOfWork)
        {
            this._readRepository = readRepository;
        }

        protected async Task<Employee> GetEmployee(string employeeId)
        {
            var employee = await _readRepository.Get(employeeId);

            if (employee is null)
            {
                throw new EntityNotFoundException<Employee>(employee?.Id);
            }

            return employee;
        }

        protected TimeoffEntry GetTimeoffEntryOrThrowException(Employee employee, Timeoff timeoff, string timeoffEntryId)
        {
            var timeoffEntry = timeoff?.GetTimeoffEntry(timeoffEntryId);
            if (timeoffEntry is null)
            {
                throw new EntityNotFoundException<TimeoffEntry>(timeoffEntryId);
            }
            return timeoffEntry;
        }

        protected Timeoff GetTimeoffOrThrowException(Employee employee, string timeoffId)
        {
            var timeoffEntry = employee.GetTimeoff(timeoffId);
            if (timeoffEntry is null)
            {
                throw new EntityNotFoundException<Timeoff>(timeoffId);
            }
            return timeoffEntry;
        }
    }
}

using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.CommandHandlers
{
    internal class AddEntryToTimeoffCommandHandler : BaseEmployeeCommandHandler<AddEntryToTimeoff>
    {
        private readonly IReadRepository<Employee> _readRepository;
        private readonly IWorkflowService _workflowService;

        public AddEntryToTimeoffCommandHandler(
            IReadRepository<Employee> readRepository,
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork) : base(readRepository, dispatcher, unitOfWork)
        {
            this._readRepository = readRepository;
            this._workflowService = workflowService;
        }

        public override async Task<IEnumerable<IDomainEvent>> HandleCore(AddEntryToTimeoff command, CancellationToken token)
        {
            var employee = await GetEmployee(command);
            var timeoffId = GetTimeoffId(command);

            var timeoff = GetTimeoffOrThrowException(employee, timeoffId);

            _workflowService.AuthorizeTransition(timeoff, TimeoffTransitions.ADD_ENTRY, timeoff.Status);

            employee.AddTimeoffEntry(command.RequestDate, command.Type, command.Hours, timeoffId);

            return LaunchedAsSubCommand ? Enumerable.Empty<IDomainEvent>() : employee.GetDomainEvents();
        }

        private async Task<Employee> GetEmployee(AddEntryToTimeoff command)
        {
            if (LaunchedAsSubCommand)
            {
                var employee = _parentCommandContext["Employee"] as Employee;
                if(employee is null)
                {
                    throw new InvalidOperationException($"Cannot call subcommand {nameof(AddEntryToTimeoff)} without providing Employee data ");
                }
                return employee;
            }
            else
            {
                return await GetEmployee(command.EmployeeId);
            }
        }

        private string GetTimeoffId(AddEntryToTimeoff command)
        {
            if (LaunchedAsSubCommand)
            {
                var timeoffId = _parentCommandContext["TimeoffId"] as string;
                if (timeoffId is null)
                {
                    throw new InvalidOperationException($"Cannot call subcommand {nameof(AddEntryToTimeoff)} without providing TimesheetId data");
                }

                return timeoffId;
            }
            else
            {
                return command.TimeoffId;
            }
        }
    }
}

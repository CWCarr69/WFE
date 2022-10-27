using Timesheet.Application.Shared;
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application
{
    public interface ICommand
    {
        public User Author { get; set; }
        public CommandActionType ActionType();
    }
}

using Timesheet.Application.Shared;

namespace Timesheet.Application
{
    public interface ICommand
    {
        public CommandActionType ActionType();
    }
}

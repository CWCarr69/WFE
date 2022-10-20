using Timesheet.Application.Shared;

namespace Timesheet.Application
{
    public interface ICommand
    {
        public string AuthorId { get; set; }
        public CommandActionType ActionType();
    }
}

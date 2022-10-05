using Timesheet.Application.Shared;

namespace Timesheet.Application.Holidays.Commands
{
    public class DeleteHoliday : ICommand
    {
        public string Id { get; set; }
        public CommandActionType ActionType() => CommandActionType.DELETION;

    }
}

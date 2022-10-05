using Timesheet.Application.Shared;

namespace Timesheet.Application.Holidays.Commands
{
    public class AddHoliday : ICommand
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsRecurrent { get; set; }
        public CommandActionType ActionType() => CommandActionType.CREATION;

    }
}

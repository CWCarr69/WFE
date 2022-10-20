using Timesheet.Application.Shared;

namespace Timesheet.Application.Holidays.Commands
{
    public class AddHoliday : BaseCommand
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsRecurrent { get; set; }
        public override CommandActionType ActionType() => CommandActionType.CREATION;
    }
}

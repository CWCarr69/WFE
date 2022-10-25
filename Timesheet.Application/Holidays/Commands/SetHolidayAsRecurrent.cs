using Timesheet.Application.Shared;

namespace Timesheet.Application.Holidays.Commands
{
    public class SetHolidayAsRecurrent : BaseCommand
    {
        public string Id { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;

    }
}

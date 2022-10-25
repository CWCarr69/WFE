using Timesheet.Application.Shared;

namespace Timesheet.Application.Holidays.Commands
{
    public class DeleteHoliday : BaseCommand
    {
        public string Id { get; set; }
        public override CommandActionType ActionType() => CommandActionType.DELETION;

    }
}

using Timesheet.Application.Shared;

namespace Timesheet.Application.Notifications.Commands
{
    public class UpdateNotification : BaseCommand
    {
        public string Id { get; set; }
        public int Population { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;
    }
}

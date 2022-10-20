using Timesheet.Application.Shared;

namespace Timesheet.Application.Settings.Commands
{
    public class UpdateSetting : BaseCommand
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public override CommandActionType ActionType() => CommandActionType.MODIFICATION;
    }
}

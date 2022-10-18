using Timesheet.Application.Shared;

namespace Timesheet.Application.Settings.Commands
{
    public class UpdateSetting : ICommand
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string Value { get; set; }
        public CommandActionType ActionType() => CommandActionType.MODIFICATION;
    }
}

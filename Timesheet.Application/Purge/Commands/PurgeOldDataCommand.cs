using Timesheet.Application.Shared;

namespace Timesheet.Application.Purge.Commands
{
  public class PurgeOldDataCommand : BaseCommand
  {
    public DateTime OlderThan { get; set; }
    public override CommandActionType ActionType() => CommandActionType.DELETION;
  }
}

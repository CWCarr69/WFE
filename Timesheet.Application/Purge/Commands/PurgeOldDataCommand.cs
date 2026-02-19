using Timesheet.Application.Shared;
using Timesheet.Domain.Models.Purge;

namespace Timesheet.Application.Purge.Commands
{
  public class PurgeOldDataCommand : BaseCommand
  {
    public DateTime OlderThan { get; set; }
    public PurgeResult? Result { get; set; }
    public override CommandActionType ActionType() => CommandActionType.DELETION;
  }
}

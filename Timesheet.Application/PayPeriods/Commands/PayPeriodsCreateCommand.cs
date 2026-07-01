using Timesheet.Application.Shared;
using Timesheet.Domain.Models.PayPeriod;

namespace Timesheet.Application.PayPeriod.Commands
{
  public class PayPeriodCreateCommand : BaseCommand
  {
    public DateTime StartFrom { get; set; }
    public PayPeriodCreateResult? Result { get; set; }
    public override CommandActionType ActionType() => CommandActionType.DELETION;
  }
}

using Timesheet.Domain.Models.PayPeriod;
using Timesheet.Domain.Models.Purge;

namespace Timesheet.Domain.Repositories
{
  public interface IPayPeriodRepository
  {
    Task<PayPeriodCreateResult> PayPeriodCreateAsync(DateTime startFron, CancellationToken cancellationToken);

  }
}

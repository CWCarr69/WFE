using Timesheet.Domain.Models.Purge;

namespace Timesheet.Domain.Repositories
{
  public interface IPurgeOldDataRepository
  {
    Task<PurgeResult> PurgeOldDataAsync(DateTime olderThan, CancellationToken cancellationToken);
  }
}

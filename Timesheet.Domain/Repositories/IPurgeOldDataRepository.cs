using Timesheet.Domain.Models.Purge;

namespace Timesheet.Domain.Repositories
{
  public interface IPurgeOldDataRepository
  {
    void PurgeOldDataAsync(DateTime olderThan, CancellationToken cancellationToken);
  }
}

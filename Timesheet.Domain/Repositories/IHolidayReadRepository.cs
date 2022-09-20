using Timesheet.Domain.Models;

namespace Timesheet.Domain.Repositories
{
    public interface IHolidayReadRepository : IReadRepository<Holiday>
    {
        Holiday? GetByDate(DateTime date);
    }
}

using Timesheet.Domain.Models;
using Timesheet.Domain.Models.Holidays;

namespace Timesheet.Domain.Repositories
{
    public interface IHolidayReadRepository : IReadRepository<Holiday>
    {
        Holiday? GetByDate(DateTime date);
    }
}

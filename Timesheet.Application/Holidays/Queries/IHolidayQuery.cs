using Timesheet.Domain.Models;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Queries
{
    public interface IHolidayQuery : IReadRepository<Holiday>
    {
        Holiday? GetByDate(DateTime date);
        IEnumerable<Holiday> GetAllHolidays(DateTime? start, DateTime? end);
    }
}

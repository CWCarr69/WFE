using Timesheet.Domain.ReadModels.Holidays;

namespace Timesheet.Application.Holidays.Queries
{
    public interface IQueryHoliday
    {
        Task<HolidayDetails?> GetDetails(string id);
        //HolidayDetails? GetByDate(DateTime date);
        Task<IEnumerable<HolidayDetails>> GetAllHolidays(DateTime? start=null, DateTime? end=null);
    }
}

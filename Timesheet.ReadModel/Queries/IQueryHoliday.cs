using Timesheet.ReadModel.ReadModels;

namespace Timesheet.ReadModel.Queries
{
    public interface IQueryHoliday
    {
        HolidayDetails? GetDetails(string id);
        HolidayDetails? GetByDate(DateTime date);
        IEnumerable<HolidayDetails> GetAllHolidays(DateTime? start=null, DateTime? end=null);
    }
}

using Timesheet.Application.Holidays.Queries;
using Timesheet.Domain.ReadModels.Holidays;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.Infrastructure.ReadModel.Queries
{
    public class QueryHoliday : IQueryHoliday
    {
        private readonly IDatabaseService _dbService;

        public QueryHoliday(IDatabaseService dbService)
        {
            this._dbService = dbService;
        }

        public async Task<HolidayDetails?> GetDetails(string id)
        {
            var idParam = "@id";
            var query = $"Select * from holidays where id = {idParam}";
            var holiday = await _dbService.QueryAsync<HolidayDetails?>(query, new { id });

            return holiday.FirstOrDefault();
        }

        public async Task<IEnumerable<HolidayDetails>> GetAllHolidays(DateTime? start = null, DateTime? end = null)
        {
            var query = "";
            object @params = null;

            if (start is null && end is null)
            {
                query = $"Select * from holidays";
            }
            else if (end is null)
            {
                var startParam = "@startDate";
                query = $"Select * from holidays where date >= {startParam}";
                @params = new { startDate = start };
            }
            else if (start is null)
            {
                var endParam = "@endDate";
                query = $"Select * from holidays where date <= {endParam}";
                @params = new { endDate = end };
            }
            else
            {
                var startParam = "@startDate";
                var endParam = "@endDate";
                query = $"Select * from holidays where date between {startParam} and {endParam}";
                @params = new { startDate = start, endDate = end };
            }

            if(@params is null)
            {
                return await _dbService.QueryAsync<HolidayDetails>(query);
            }
            else
            {
                return await _dbService.QueryAsync<HolidayDetails>(query, @params);
            }
        }
    }
}
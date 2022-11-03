using Timesheet.Application.Settings.Queries;
using Timesheet.Domain.ReadModels.Settings;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.Infrastructure.ReadModel.Queries
{
    public class QuerySetting : IQuerySetting
    {
        private readonly IDatabaseService _dbService;

        public QuerySetting(IDatabaseService dbService)
        {
            this._dbService = dbService;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        public async Task<IEnumerable<SettingDetailsGrouped>> GetSettings()
        {
            var query = $@"SELECT
                        Id AS {nameof(SettingDetails.Id)},
                        Name AS {nameof(SettingDetails.Name)},
                        Value AS {nameof(SettingDetails.Value)},
                        Description AS {nameof(SettingDetails.Description)},
                        LEFT(Name, CASE WHEN charindex('_', Name) = 0
                        THEN
                        LEN(Name) 
                        ELSE charindex('_', Name) - 1 END) AS [{nameof(SettingDetails.Group)}]
                        FROM settings 
                        ORDER by name";
            var settings = await _dbService.QueryAsync<SettingDetails>(query);

            var groupedSettings = settings.GroupBy(s => s.Group)
                .Select(g => new SettingDetailsGrouped
                {
                    Group = g.Key,
                    Settings = g
                });

            return groupedSettings;
        }

        public async Task<SettingDetails> GetSetting(string name)
        {
            var nameParam = "@name";
            var query = $"Select * from settings where name = {nameParam}";
            var setting = (await _dbService.QueryAsync<SettingDetails>(query, new {name}))
                ?.FirstOrDefault();

            return setting;
        }
    }
}
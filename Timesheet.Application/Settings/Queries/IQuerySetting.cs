using Timesheet.Domain.ReadModels.Settings;

namespace Timesheet.Application.Settings.Queries
{
    public interface IQuerySetting
    {
        Task<IEnumerable<SettingDetails>> GetSettings();
        Task<SettingDetails> GetSetting(string name);
    }
}

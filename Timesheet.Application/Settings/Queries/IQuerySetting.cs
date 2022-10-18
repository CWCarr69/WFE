using Timesheet.Domain.Models.Settings;
using Timesheet.Domain.ReadModels.Settings;

namespace Timesheet.Application.Holidays.Queries
{
    public interface IQuerySetting
    {
        Task<IEnumerable<SettingDetails>> GetSettings();
    }
}


using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Services;

internal class SettingRepository: ISettingRepository
{
    private readonly IDatabaseService _dbServices;

    public SettingRepository(IDatabaseService dbServices)
    {
        _dbServices = dbServices;
    }

    FDPSettings ISettingRepository.GetFDPParameters()
    {
        var query = "SELECT * FROM Settings WHERE name LIKE 'FDP_%'";
        var settings = _dbServices.Query<(string Name, string Value)>(query);

        if (settings is null)
        {
            throw new Exception("FDP settings are not complete");
        }
        else {
            return FDPSettings.CreateFromConfigurationList(settings);
        }
    }
}

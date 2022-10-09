using Timesheet.EmailSender.Models;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailSender.Repositories;

internal class SettingRepository: ISettingRepository
{
    private readonly IDatabaseService _dbServices;

    public SettingRepository(IDatabaseService dbServices)
    {
        _dbServices = dbServices;
    }

    SMTPSettings ISettingRepository.GetSMTPParameters()
    {
        var query = "SELECT * FROM Settings WHERE name LIKE 'SMTP%'";
        var settings = _dbServices.Query<(string Name, string Value)>(query);

        if(settings is null)
        {
            throw new Exception("Mail Notifications settings are not complete");
        }



        return SMTPSettings.CreateFromConfigurationList(settings);
    }
}
